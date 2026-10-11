            using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Software
{
    [Serializable]
    public class ReporteLinea
    {
        public string Categoria { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Monto { get; set; }
    }

    public partial class CalcularBalance : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Establecer fecha por defecto como fecha actual
                txtFechaReporte.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnCalcular_Click(object sender, EventArgs e)
        {
            // Validación de fecha
            if (string.IsNullOrWhiteSpace(txtFechaReporte.Text))
            {
                MostrarMensaje("Por favor, ingresa una fecha para el reporte.", true);
                return;
            }

            DateTime fechaReporte;
            if (!DateTime.TryParse(txtFechaReporte.Text, out fechaReporte))
            {
                MostrarMensaje("Por favor, ingresa una fecha válida en formato correcto.", true);
                return;
            }

            // Validar que la fecha no sea futura
            if (fechaReporte > DateTime.Now)
            {
                MostrarMensaje("La fecha del reporte no puede ser futura.", true);
                return;
            }

            List<ReporteLinea> lineasReporte = new List<ReporteLinea>();
            decimal totalActivo = 0, totalPasivo = 0, totalPatrimonioBase = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // --- Filtro por usuario (mismo criterio que en Menú y Libro Mayor) ---
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;

                    // --- CONSULTA 1 CORREGIDA: Saldos de Activo, Pasivo y Patrimonio ---
                    // CAMBIO REALIZADO: Se movió la condición de fecha al WHERE y se cambiaron los JOINs
                    // para asegurar que solo sume transacciones que realmente existen en ese rango de fecha.
                    string queryBalance = @"
                SELECT CC.CuentaID, CC.NombreCuenta, CC.TipoCuenta,
                        ISNULL(SUM(DA.Debe), 0) AS TotalDebe, 
                        ISNULL(SUM(DA.Haber), 0) AS TotalHaber
                FROM CatalogoCuentas CC
                INNER JOIN DetalleAsientos DA ON CC.CuentaID = DA.CuentaID
                INNER JOIN AsientosDiario AD ON DA.AsientoID = AD.AsientoID
                WHERE CC.TipoCuenta IN ('Activo', 'Pasivo', 'Patrimonio')
                  AND AD.FechaAsiento <= @FechaReporte
                  AND (@EsAdmin = 1 OR AD.UsuarioID = @UsuarioID)
                GROUP BY CC.CuentaID, CC.NombreCuenta, CC.TipoCuenta
                HAVING (ISNULL(SUM(DA.Debe), 0) - ISNULL(SUM(DA.Haber), 0)) != 0
                    OR (ISNULL(SUM(DA.Haber), 0) - ISNULL(SUM(DA.Debe), 0)) != 0
                ORDER BY CC.TipoCuenta, CC.NombreCuenta;";

                    using (SqlCommand cmd = new SqlCommand(queryBalance, con))
                    {
                        cmd.Parameters.AddWithValue("@FechaReporte", fechaReporte);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tipoCuenta = reader["TipoCuenta"].ToString();
                                string nombreCuenta = reader["NombreCuenta"].ToString();
                                decimal debe = (decimal)reader["TotalDebe"];
                                decimal haber = (decimal)reader["TotalHaber"];
                                decimal saldo = 0;

                                // CORREGIDO: Cálculo de saldos según naturaleza de las cuentas contables
                                if (tipoCuenta == "Activo") 
                                { 
                                    // Activos: Naturaleza deudora (Saldo = Debe - Haber)
                                    saldo = debe - haber; 
                                    totalActivo += saldo; // Sumar el saldo real (puede incluir activos negativos como provisiones)
                                }
                                else if (tipoCuenta == "Pasivo") 
                                { 
                                    // Pasivos: Naturaleza acreedora (Saldo = Haber - Debe)
                                    saldo = haber - debe; 
                                    totalPasivo += saldo; // Sumar el saldo real
                                }
                                else if (tipoCuenta == "Patrimonio") 
                                { 
                                    // Patrimonio: Naturaleza acreedora (Saldo = Haber - Debe)
                                    saldo = haber - debe; 
                                    totalPatrimonioBase += saldo; // Sumar el saldo real
                                }

                                // Agregar a la lista solo si el saldo es diferente de cero
                                if (Math.Abs(saldo) > 0.01m)
                                {
                                    lineasReporte.Add(new ReporteLinea { Categoria = tipoCuenta, NombreCuenta = nombreCuenta, Monto = saldo });
                                }
                            }
                        }
                    }

                    DateTime fechaInicioAnio = new DateTime(fechaReporte.Year, 1, 1);
                    decimal utilidadEjercicio = 0, totalIngresos = 0, totalGastos = 0;
                    decimal resultadosAcumulados = 0;

                    // --- CONSULTA 2: Utilidad del Ejercicio (Año Actual) ---
                    // CORREGIDO: Suma correctamente todas las cuentas de Ingreso y Gasto del período
                    string queryResultados = @"
                SELECT CC.TipoCuenta, 
                       ISNULL(SUM(DA.Debe), 0) AS TotalDebe, 
                       ISNULL(SUM(DA.Haber), 0) AS TotalHaber
                FROM CatalogoCuentas CC
                INNER JOIN DetalleAsientos DA ON CC.CuentaID = DA.CuentaID
                INNER JOIN AsientosDiario AD ON DA.AsientoID = AD.AsientoID
                WHERE CC.TipoCuenta IN ('Ingreso', 'Gasto') 
                  AND AD.FechaAsiento BETWEEN @FechaInicioAnio AND @FechaReporte
                  AND (@EsAdmin = 1 OR AD.UsuarioID = @UsuarioID)
                GROUP BY CC.TipoCuenta;";

                    using (SqlCommand cmd = new SqlCommand(queryResultados, con))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicioAnio", fechaInicioAnio);
                        cmd.Parameters.AddWithValue("@FechaReporte", fechaReporte);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tipoCuenta = reader["TipoCuenta"].ToString();
                                decimal debe = (decimal)reader["TotalDebe"];
                                decimal haber = (decimal)reader["TotalHaber"];
                                
                                if (tipoCuenta == "Ingreso")
                                {
                                    // Ingresos: Naturaleza acreedora - Saldo = Haber - Debe
                                    // La consulta agrupa por TipoCuenta, así que esto suma todos los ingresos
                                    decimal saldoIngresos = haber - debe;
                                    totalIngresos += saldoIngresos; // Sumar siempre (puede ser positivo o negativo)
                                }
                                else if (tipoCuenta == "Gasto")
                                {
                                    // Gastos: Naturaleza deudora - Saldo = Debe - Haber
                                    // La consulta agrupa por TipoCuenta, así que esto suma todos los gastos
                                    decimal saldoGastos = debe - haber;
                                    totalGastos += saldoGastos; // Sumar siempre (puede ser positivo o negativo)
                                }
                            }
                        }
                    }
                    // CORREGIDO: Calcular utilidad del ejercicio (siempre agregar, incluso si es negativa)
                    utilidadEjercicio = totalIngresos - totalGastos;
                    // Siempre agregar la utilidad del ejercicio, incluso si es pérdida (negativa)
                    lineasReporte.Add(new ReporteLinea { Categoria = "Patrimonio", NombreCuenta = "Utilidad del Ejercicio", Monto = utilidadEjercicio });

                    // --- CONSULTA 3: Resultados Acumulados (Años Anteriores) ---
                    // CORREGIDO: Cálculo consistente con la utilidad del ejercicio
                    decimal totalIngresosAcumulados = 0;
                    decimal totalGastosAcumulados = 0;
                    
                    string queryAcumulados = @"
                SELECT CC.TipoCuenta, 
                       ISNULL(SUM(DA.Debe), 0) AS TotalDebe, 
                       ISNULL(SUM(DA.Haber), 0) AS TotalHaber
                FROM CatalogoCuentas CC
                INNER JOIN DetalleAsientos DA ON CC.CuentaID = DA.CuentaID
                INNER JOIN AsientosDiario AD ON DA.AsientoID = AD.AsientoID
                WHERE CC.TipoCuenta IN ('Ingreso', 'Gasto') 
                  AND AD.FechaAsiento < @FechaInicioAnio
                  AND (@EsAdmin = 1 OR AD.UsuarioID = @UsuarioID)
                GROUP BY CC.TipoCuenta;";

                    using (SqlCommand cmd = new SqlCommand(queryAcumulados, con))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicioAnio", fechaInicioAnio);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tipoCuenta = reader["TipoCuenta"].ToString();
                                decimal debe = (decimal)reader["TotalDebe"];
                                decimal haber = (decimal)reader["TotalHaber"];
                                
                                if (tipoCuenta == "Ingreso")
                                {
                                    // Ingresos: Naturaleza acreedora - Saldo = Haber - Debe
                                    decimal saldoIngresos = haber - debe;
                                    totalIngresosAcumulados += saldoIngresos; // Sumar siempre para resultados acumulados
                                }
                                else if (tipoCuenta == "Gasto")
                                {
                                    // Gastos: Naturaleza deudora - Saldo = Debe - Haber
                                    decimal saldoGastos = debe - haber;
                                    totalGastosAcumulados += saldoGastos; // Sumar siempre para resultados acumulados
                                }
                            }
                        }
                    }
                    // CORREGIDO: Calcular resultados acumulados
                    resultadosAcumulados = totalIngresosAcumulados - totalGastosAcumulados;
                    // Solo agregar resultados acumulados si hay un valor significativo (diferente de cero)
                    if (Math.Abs(resultadosAcumulados) > 0.01m)
                    {
                        lineasReporte.Add(new ReporteLinea { Categoria = "Patrimonio", NombreCuenta = "Resultados Acumulados", Monto = resultadosAcumulados });
                    }

                    // --- Cálculo Final ---
                    decimal totalPatrimonio = totalPatrimonioBase + utilidadEjercicio + resultadosAcumulados;
                    decimal totalPasivoPatrimonio = totalPasivo + totalPatrimonio;

                    // Validar que haya datos para mostrar
                    if (lineasReporte.Count == 0)
                    {
                        MostrarMensaje("No se encontraron transacciones para la fecha seleccionada.", true);
                        ltlBalanceReporte.Text = "";
                        btnGuardarReporte.Visible = false;
                        return;
                    }

                    GenerarReporteHTML(lineasReporte, totalActivo, totalPasivo, totalPatrimonio, totalPasivoPatrimonio);
                    Session["NuevoBalanceDetalle"] = lineasReporte;
                    Session["NuevoBalanceFecha"] = fechaReporte;
                    btnGuardarReporte.Visible = true;
                    MostrarMensaje("Cálculo completado. Revisa el reporte y guárdalo.", false);
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al calcular: " + ex.Message, true);
                    ltlBalanceReporte.Text = "";
                    btnGuardarReporte.Visible = false;
                }
            }
        }

        protected void btnGuardarReporte_Click(object sender, EventArgs e)
        {
            // Validaciones de sesión
            if (Session["UsuarioID"] == null)
            {
                MostrarMensaje("Error: Tu sesión ha expirado. Por favor, inicia sesión nuevamente.", true);
                Response.Redirect("Login.aspx");
                return;
            }

            var lineasReporte = Session["NuevoBalanceDetalle"] as List<ReporteLinea>;
            if (lineasReporte == null || Session["NuevoBalanceFecha"] == null)
            {
                MostrarMensaje("Error: La sesión ha expirado. Por favor, calcula el balance de nuevo.", true);
                btnGuardarReporte.Visible = false;
                return;
            }

            if (lineasReporte.Count == 0)
            {
                MostrarMensaje("Error: No hay datos para guardar. Por favor, calcula el balance primero.", true);
                return;
            }

            DateTime fechaReporte = (DateTime)Session["NuevoBalanceFecha"];
            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);
            bool exitoGuardado = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    string queryHeader = "INSERT INTO BalancesGenerales (FechaReporte, UsuarioID) VALUES (@Fecha, @UsuarioID); SELECT SCOPE_IDENTITY();";
                    int nuevoBalanceID;

                    using (SqlCommand cmdHeader = new SqlCommand(queryHeader, con, tran))
                    {
                        cmdHeader.Parameters.AddWithValue("@Fecha", fechaReporte);
                        cmdHeader.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        nuevoBalanceID = Convert.ToInt32(cmdHeader.ExecuteScalar());
                    }

                    string queryDetail = "INSERT INTO DetalleBalance (BalanceID, CategoriaCuenta, NombreCuenta, Monto) VALUES (@BalanceID, @Categoria, @NombreCuenta, @Monto)";

                    using (SqlCommand cmdDetail = new SqlCommand(queryDetail, con, tran))
                    {
                        foreach (var linea in lineasReporte)
                        {
                            cmdDetail.Parameters.Clear();
                            cmdDetail.Parameters.AddWithValue("@BalanceID", nuevoBalanceID);
                            cmdDetail.Parameters.AddWithValue("@Categoria", linea.Categoria);
                            cmdDetail.Parameters.AddWithValue("@NombreCuenta", linea.NombreCuenta);
                            cmdDetail.Parameters.AddWithValue("@Monto", linea.Monto);

                            cmdDetail.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    exitoGuardado = true;
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); } catch { }
                    MostrarMensaje("Error al guardar el reporte: " + ex.Message, true);
                }
            }

            if (exitoGuardado)
            {
                Session["NuevoBalanceDetalle"] = null;
                Session["NuevoBalanceFecha"] = null;
                Response.Redirect("BalanceGeneral.aspx");
            }
        }

        private void GenerarReporteHTML(List<ReporteLinea> lineas, decimal totalActivo, decimal totalPasivo, decimal totalPatrimonio, decimal totalPasivoPatrimonio)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='reporte-container'>");
            sb.Append("<div class='reporte-content'>");

            // Sección Activos
            sb.Append("<h3>Activos</h3>");
            sb.Append("<div class='reporte-seccion'>");
            foreach (var linea in lineas.Where(l => l.Categoria == "Activo"))
            {
                string valorFormateado = linea.Monto.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
                sb.AppendFormat("<div class='linea-item'><span class='linea-nombre'>{0}</span><span class='linea-valor'>{1}</span></div>", 
                    linea.NombreCuenta, valorFormateado);
            }
            string totalActivoFormateado = totalActivo.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
            sb.AppendFormat("<div class='linea-total'><span class='linea-nombre'>TOTAL ACTIVOS</span><span class='linea-valor'>{0}</span></div>", totalActivoFormateado);
            sb.Append("</div>");

            // Sección Pasivos
            sb.Append("<h3 style='margin-top: 2.5rem;'>Pasivos</h3>");
            sb.Append("<div class='reporte-seccion'>");
            foreach (var linea in lineas.Where(l => l.Categoria == "Pasivo"))
            {
                string valorFormateado = linea.Monto.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
                sb.AppendFormat("<div class='linea-item'><span class='linea-nombre'>{0}</span><span class='linea-valor'>{1}</span></div>", 
                    linea.NombreCuenta, valorFormateado);
            }
            string totalPasivoFormateado = totalPasivo.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
            sb.AppendFormat("<div class='linea-total'><span class='linea-nombre'>TOTAL PASIVOS</span><span class='linea-valor'>{0}</span></div>", totalPasivoFormateado);
            sb.Append("</div>");

            // Sección Patrimonio
            sb.Append("<h3 style='margin-top: 2.5rem;'>Patrimonio</h3>");
            sb.Append("<div class='reporte-seccion'>");
            foreach (var linea in lineas.Where(l => l.Categoria == "Patrimonio"))
            {
                string valorFormateado = linea.Monto.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
                sb.AppendFormat("<div class='linea-item'><span class='linea-nombre'>{0}</span><span class='linea-valor'>{1}</span></div>", 
                    linea.NombreCuenta, valorFormateado);
            }
            string totalPatrimonioFormateado = totalPatrimonio.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
            sb.AppendFormat("<div class='linea-total'><span class='linea-nombre'>TOTAL PATRIMONIO</span><span class='linea-valor'>{0}</span></div>", totalPatrimonioFormateado);
            sb.Append("</div>");

            // Total Pasivo + Patrimonio
            string totalPasivoPatrimonioFormateado = totalPasivoPatrimonio.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
            sb.AppendFormat("<div class='linea-total-final'><span class='linea-nombre'>TOTAL PASIVO + PATRIMONIO</span><span class='linea-valor'>{0}</span></div>", totalPasivoPatrimonioFormateado);

            // Validación de ecuación patrimonial
            if (Math.Abs(totalActivo - totalPasivoPatrimonio) < 0.01m)
            {
                sb.Append("<h4 class='ecuacion-ok'><svg class='icon-finance-inline' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='#059669' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M20 6L9 17l-5-5'/></svg>ECUACIÓN PATRIMONIAL CUMPLIDA (ACTIVO = PASIVO + PATRIMONIO)</h4>");
            }
            else
            {
                sb.Append("<h4 class='ecuacion-error'><svg class='icon-finance-inline' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='#d97706' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z'/><line x1='12' y1='9' x2='12' y2='13'/><line x1='12' y1='17' x2='12.01' y2='17'/></svg>¡DESCUADRE! EL BALANCE NO CUMPLE LA ECUACIÓN PATRIMONIAL</h4>");
            }

            sb.Append("</div>"); // cierra reporte-content
            sb.Append("</div>"); // cierra reporte-container
            ltlBalanceReporte.Text = sb.ToString();
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}