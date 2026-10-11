using System;
using System.Collections.Generic; // Para guardar las líneas del reporte
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text; // Para construir el reporte HTML
using System.Web.UI;
using System.Web.UI.WebControls; // Para el DropDownList
using System.Linq;

namespace Software
{
    // Una clase simple para guardar las líneas del reporte de Origen/Aplicación
    [Serializable]
    public class LineaFlujo
    {
        public string CategoriaFlujo { get; set; } // "Origen" o "Aplicacion"
        public string NombreLinea { get; set; }
        public decimal Monto { get; set; }
    }

    // Una clase para guardar los saldos de un balance
    public class SaldoCuenta
    {
        public string NombreCuenta { get; set; }
        public string TipoCuenta { get; set; } // Activo, Pasivo, Patrimonio
        public decimal Saldo { get; set; }
    }


    public partial class CalcularEstadoOrigen : System.Web.UI.Page
    {
        // Leer la cadena de conexión
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- VERIFICACIÓN DE SEGURIDAD ---
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                // NUEVA FUNCIÓN: Llenar los DropDownLists con balances guardados
                PoblarBalancesGuardados();
            }
        }

        /// <summary>
        /// NUEVA FUNCIÓN: Lee BalancesGenerales y los carga en los DDL.
        /// </summary>
        private void PoblarBalancesGuardados()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Consulta para obtener los reportes guardados
                    string query = @"SELECT 
                                        BalanceID, 
                                        'ID ' + CAST(BalanceID AS VARCHAR(10)) + 
                                        ' - Reporte del ' + CONVERT(VARCHAR, FechaReporte, 103) +
                                        ' (Calculado el ' + CONVERT(VARCHAR, FechaGeneracion, 103) + ')'
                                        AS ReporteInfo
                                     FROM BalancesGenerales 
                                     ORDER BY FechaReporte DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Llenar el primer DDL
                    ddlBalanceInicial.DataSource = dt;
                    ddlBalanceInicial.DataTextField = "ReporteInfo";
                    ddlBalanceInicial.DataValueField = "BalanceID";
                    ddlBalanceInicial.DataBind();
                    ddlBalanceInicial.Items.Insert(0, new ListItem("-- Seleccione Balance Inicial --", "0"));

                    // Llenar el segundo DDL (con la misma data)
                    ddlBalanceFinal.DataSource = dt;
                    ddlBalanceFinal.DataTextField = "ReporteInfo";
                    ddlBalanceFinal.DataValueField = "BalanceID";
                    ddlBalanceFinal.DataBind();
                    ddlBalanceFinal.Items.Insert(0, new ListItem("-- Seleccione Balance Final --", "0"));
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar balances guardados: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// LÓGICA MODIFICADA: Ahora lee los IDs de los DDL.
        /// </summary>
        protected void btnCalcular_Click(object sender, EventArgs e)
        {
            int balanceID_Inicio = Convert.ToInt32(ddlBalanceInicial.SelectedValue);
            int balanceID_Fin = Convert.ToInt32(ddlBalanceFinal.SelectedValue);

            // --- Validaciones ---
            if (balanceID_Inicio == 0 || balanceID_Fin == 0)
            {
                MostrarMensaje("Debe seleccionar ambos balances (Inicial y Final).", true);
                return;
            }
            if (balanceID_Inicio == balanceID_Fin)
            {
                MostrarMensaje("Debe seleccionar dos balances diferentes para comparar.", true);
                return;
            }

            try
            {
                // 1. Cargar los dos balances GUARDADOS
                Dictionary<string, SaldoCuenta> saldosInicio = CargarSaldosGuardados(balanceID_Inicio);
                Dictionary<string, SaldoCuenta> saldosFin = CargarSaldosGuardados(balanceID_Fin);

                // 2. Obtener las fechas de esos reportes para validación y guardado
                DateTime fechaInicio = ObtenerFechaReporte(balanceID_Inicio);
                DateTime fechaFin = ObtenerFechaReporte(balanceID_Fin);

                if (fechaInicio >= fechaFin)
                {
                    MostrarMensaje("La fecha del Balance Final debe ser posterior a la del Balance Inicial.", true);
                    return;
                }

                // 3. Comparar los balances para obtener variaciones
                List<LineaFlujo> lineasReporte = CompararBalances(saldosInicio, saldosFin);

                // 4. Generar el reporte HTML
                GenerarReporteHTML(lineasReporte);

                // 5. Guardar en Sesión para el botón "Guardar"
                Session["NuevoEstadoFlujoDetalle"] = lineasReporte;
                Session["NuevoEstadoFlujoFechaInicio"] = fechaInicio;
                Session["NuevoEstadoFlujoFechaFin"] = fechaFin;

                btnGuardarReporte.Visible = true;
                MostrarMensaje("Cálculo por comparación completado. Revisa el reporte y guárdalo.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al calcular: " + ex.Message, true);
                ltlReporte.Text = "";
                btnGuardarReporte.Visible = false;
            }
        }

        /// <summary>
        /// NUEVA FUNCIÓN: Obtiene los detalles de un balance YA GUARDADO.
        /// </summary>
        private Dictionary<string, SaldoCuenta> CargarSaldosGuardados(int balanceID)
        {
            var saldos = new Dictionary<string, SaldoCuenta>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT NombreCuenta, CategoriaCuenta, Monto FROM DetalleBalance WHERE BalanceID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", balanceID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombreCuenta = reader["NombreCuenta"].ToString();
                            string tipoCuenta = reader["CategoriaCuenta"].ToString();
                            decimal monto = (decimal)reader["Monto"];

                            saldos[nombreCuenta] = new SaldoCuenta { NombreCuenta = nombreCuenta, TipoCuenta = tipoCuenta, Saldo = monto };
                        }
                    }
                }
            }
            return saldos;
        }

        /// <summary>
        /// NUEVA FUNCIÓN ASISTENTE: Obtiene la fecha de un reporte guardado.
        /// </summary>
        private DateTime ObtenerFechaReporte(int balanceID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT FechaReporte FROM BalancesGenerales WHERE BalanceID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", balanceID);
                    return (DateTime)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// FUNCIÓN ELIMINADA: Ya no calculamos desde cero.
        /// private Dictionary<string, SaldoCuenta> CalcularSaldosBalance(DateTime fechaReporte) { ... }
        /// </summary>

        /// <summary>
        /// FUNCIÓN REUTILIZADA: Compara los dos balances y genera la lista de Orígenes y Aplicaciones.
        /// (Esta función no necesita cambios)
        /// </summary>
        private List<LineaFlujo> CompararBalances(Dictionary<string, SaldoCuenta> saldosInicio, Dictionary<string, SaldoCuenta> saldosFin)
        {
            var lineasReporte = new List<LineaFlujo>();
            var todasLasCuentas = saldosInicio.Keys.Union(saldosFin.Keys);

            foreach (var nombreCuenta in todasLasCuentas)
            {
                decimal saldoInicial = saldosInicio.ContainsKey(nombreCuenta) ? saldosInicio[nombreCuenta].Saldo : 0;
                decimal saldoFinal = saldosFin.ContainsKey(nombreCuenta) ? saldosFin[nombreCuenta].Saldo : 0;
                string tipoCuenta = saldosFin.ContainsKey(nombreCuenta) ? saldosFin[nombreCuenta].TipoCuenta : (saldosInicio.ContainsKey(nombreCuenta) ? saldosInicio[nombreCuenta].TipoCuenta : "Activo"); // Default a Activo si es cuenta nueva

                decimal variacion = saldoFinal - saldoInicial;
                if (variacion == 0) continue;

                string categoriaFlujo = "";
                string nombreLinea = "Variación en " + nombreCuenta;
                decimal montoAbsoluto = Math.Abs(variacion);

                if (tipoCuenta == "Activo")
                {
                    if (variacion > 0) categoriaFlujo = "Aplicacion"; // Aumento de Activo es un USO
                    else categoriaFlujo = "Origen";                  // Disminución de Activo es una FUENTE
                }
                else if (tipoCuenta == "Pasivo" || tipoCuenta == "Patrimonio")
                {
                    if (variacion > 0) categoriaFlujo = "Origen";     // Aumento de Pasivo/Patrimonio es una FUENTE
                    else categoriaFlujo = "Aplicacion";               // Disminución de Pasivo/Patrimonio es un USO
                }

                if (categoriaFlujo != "")
                {
                    lineasReporte.Add(new LineaFlujo { CategoriaFlujo = categoriaFlujo, NombreLinea = nombreLinea, Monto = montoAbsoluto });
                }
            }
            return lineasReporte;
        }


        /// <summary>
        /// FUNCIÓN MODIFICADA: Corregido bug de @Nombre -> @NombreLinea
        /// </summary>
        protected void btnGuardarReporte_Click(object sender, EventArgs e)
        {
            var lineasReporte = Session["NuevoEstadoFlujoDetalle"] as List<LineaFlujo>;
            if (lineasReporte == null || Session["NuevoEstadoFlujoFechaInicio"] == null || Session["UsuarioID"] == null)
            {
                MostrarMensaje("Error: La sesión ha expirado. Por favor, calcula el estado de nuevo.", true);
                return;
            }

            DateTime fechaInicio = (DateTime)Session["NuevoEstadoFlujoFechaInicio"];
            DateTime fechaFin = (DateTime)Session["NuevoEstadoFlujoFechaFin"];
            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);
            bool exitoGuardado = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    string queryHeader = "INSERT INTO EstadosFlujo (FechaInicio, FechaFin, UsuarioID) VALUES (@Inicio, @Fin, @UsuarioID); SELECT SCOPE_IDENTITY();";
                    int nuevoEstadoID;

                    using (SqlCommand cmdHeader = new SqlCommand(queryHeader, con, tran))
                    {
                        cmdHeader.Parameters.AddWithValue("@Inicio", fechaInicio);
                        cmdHeader.Parameters.AddWithValue("@Fin", fechaFin);
                        cmdHeader.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        nuevoEstadoID = Convert.ToInt32(cmdHeader.ExecuteScalar());
                    }

                    // --- CORRECCIÓN DE BUG ---
                    string queryDetail = "INSERT INTO DetalleEstadoFlujo (EstadoFlujoID, CategoriaFlujo, NombreLinea, Monto) VALUES (@ID, @Categoria, @NombreLinea, @Monto)";

                    using (SqlCommand cmdDetail = new SqlCommand(queryDetail, con, tran))
                    {
                        foreach (var linea in lineasReporte)
                        {
                            cmdDetail.Parameters.Clear();
                            cmdDetail.Parameters.AddWithValue("@ID", nuevoEstadoID);
                            cmdDetail.Parameters.AddWithValue("@Categoria", linea.CategoriaFlujo);
                            cmdDetail.Parameters.AddWithValue("@NombreLinea", linea.NombreLinea); // Coincide con la consulta
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
                Session["NuevoEstadoFlujoDetalle"] = null;
                Session["NuevoEstadoFlujoFechaInicio"] = null;
                Session["NuevoEstadoFlujoFechaFin"] = null;
                Response.Redirect("EstadoOrigenAplicacion.aspx");
            }
        }


        // --- FUNCIONES ASISTENTE (Sin cambios) ---

        private void GenerarReporteHTML(List<LineaFlujo> lineas)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='reporte-container'>");

            decimal totalOrigen = 0;
            decimal totalAplicacion = 0;

            // --- Columna de Orígenes ---
            sb.Append("<div class='reporte-columna'>");
            sb.Append("<h3>Orígenes (Fuentes)</h3>");
            foreach (var linea in lineas.Where(l => l.CategoriaFlujo == "Origen"))
            {
                sb.AppendFormat("<div class='linea-item'><span>{0}</span><span>{1:N2}</span></div>", linea.NombreLinea, linea.Monto);
                totalOrigen += linea.Monto;
            }
            sb.AppendFormat("<div class='linea-total'><span>TOTAL ORÍGENES</span><span>{0:N2}</span></div>", totalOrigen);
            sb.Append("</div>");

            // --- Columna de Aplicaciones ---
            sb.Append("<div class='reporte-columna'>");
            sb.Append("<h3>Aplicaciones (Usos)</h3>");
            foreach (var linea in lineas.Where(l => l.CategoriaFlujo == "Aplicacion"))
            {
                sb.AppendFormat("<div class='linea-item'><span>{0}</span><span>{1:N2}</span></div>", linea.NombreLinea, linea.Monto);
                totalAplicacion += linea.Monto;
            }
            sb.AppendFormat("<div class='linea-total'><span>TOTAL APLICACIONES</span><span>{0:N2}</span></div>", totalAplicacion);
            sb.Append("</div>");

            // --- Verificación Final ---
            if (Math.Abs(totalOrigen - totalAplicacion) < 0.01m)
            {
                sb.Append("<h4 class='reporte-balance-ok'>BALANCE CUMPLIDO (Total Orígenes = Total Aplicaciones)</h4>");
            }

            sb.Append("</div>");
            ltlReporte.Text = sb.ToString();
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}