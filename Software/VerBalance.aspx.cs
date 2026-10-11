using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;

namespace Software
{
    public partial class VerBalance : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null) Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null && int.TryParse(Request.QueryString["ID"], out int balanceID))
                {
                    CargarDetalleBalance(balanceID);
                }
                else
                {
                    lblMensaje.Text = "ID de reporte inválido.";
                    lblMensaje.Visible = true;
                }
            }
        }

        private void CargarDetalleBalance(int balanceID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Verificar permisos: solo admin puede ver todos, usuarios regulares solo los suyos
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    // 1. Obtener datos del Encabezado con información del usuario
                    string queryHeader = @"SELECT 
                                            B.FechaReporte, 
                                            B.FechaGeneracion,
                                            U.NombreCompleto AS Autor,
                                            U.NombreUsuario,
                                            B.BalanceID,
                                            B.UsuarioID
                                           FROM BalancesGenerales B
                                           INNER JOIN Usuarios U ON B.UsuarioID = U.UsuarioID
                                           WHERE B.BalanceID = @ID";
                    
                    DateTime fechaReporte = DateTime.MinValue;
                    DateTime fechaGeneracion = DateTime.MinValue;
                    string autor = "";
                    string nombreUsuario = "";
                    int balanceId = 0;
                    int balanceUsuarioID = 0;

                    using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", balanceID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                balanceUsuarioID = (int)r["UsuarioID"];
                                
                                // Verificar permisos
                                if (!esAdmin && balanceUsuarioID != usuarioID)
                                {
                                    lblMensaje.Text = "No tienes permiso para ver este reporte.";
                                    lblMensaje.CssClass = "feedback-message error";
                                    lblMensaje.Visible = true;
                                    return;
                                }
                                
                                fechaReporte = (DateTime)r["FechaReporte"];
                                fechaGeneracion = (DateTime)r["FechaGeneracion"];
                                autor = r["Autor"].ToString();
                                nombreUsuario = r["NombreUsuario"].ToString();
                                balanceId = (int)r["BalanceID"];
                            }
                            else 
                            { 
                                lblMensaje.Text = "Reporte no encontrado."; 
                                lblMensaje.Visible = true; 
                                return; 
                            }
                        }
                    }

                    // 2. Obtener las líneas del Detalle con información de cuenta del catálogo si existe
                    string queryDetail = @"SELECT DB.CategoriaCuenta, DB.NombreCuenta, DB.Monto, 
                                          CC.CodigoCuenta, CC.TipoCuenta
                                          FROM DetalleBalance DB
                                          LEFT JOIN CatalogoCuentas CC ON DB.NombreCuenta = CC.NombreCuenta
                                          WHERE DB.BalanceID = @ID 
                                          ORDER BY 
                                            CASE DB.CategoriaCuenta 
                                                WHEN 'Activo' THEN 1 
                                                WHEN 'Pasivo' THEN 2 
                                                WHEN 'Patrimonio' THEN 3 
                                                ELSE 4 END,
                                            CC.CodigoCuenta,
                                            DB.NombreCuenta";
                    SqlDataAdapter da = new SqlDataAdapter(queryDetail, con);
                    da.SelectCommand.Parameters.AddWithValue("@ID", balanceID);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // 3. Obtener estadísticas y comparaciones
                    DataTable estadisticas = ObtenerEstadisticas(con, balanceID, fechaReporte);

                    // 4. Construir el HTML con más detalles
                    GenerarHTML(fechaReporte, fechaGeneracion, autor, nombreUsuario, balanceId, dt, estadisticas);
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
                    lblMensaje.Visible = true;
                }
            }
        }

        private DataTable ObtenerEstadisticas(SqlConnection con, int balanceID, DateTime fechaReporte)
        {
            DataTable dt = new DataTable();
            
            try
            {
                // Obtener balance anterior para comparación
                string queryAnterior = @"SELECT TOP 1 BalanceID, FechaReporte 
                                         FROM BalancesGenerales 
                                         WHERE FechaReporte < @FechaReporte 
                                         ORDER BY FechaReporte DESC";
                
                int? balanceAnteriorID = null;
                using (SqlCommand cmd = new SqlCommand(queryAnterior, con))
                {
                    cmd.Parameters.AddWithValue("@FechaReporte", fechaReporte);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        balanceAnteriorID = (int)result;
                }

                // Contar cuentas por categoría
                string queryStats = @"SELECT 
                                        CategoriaCuenta,
                                        COUNT(*) AS CantidadCuentas,
                                        SUM(Monto) AS TotalCategoria
                                      FROM DetalleBalance 
                                      WHERE BalanceID = @ID
                                      GROUP BY CategoriaCuenta";
                
                SqlDataAdapter da = new SqlDataAdapter(queryStats, con);
                da.SelectCommand.Parameters.AddWithValue("@ID", balanceID);
                da.Fill(dt);
            }
            catch { }

            return dt;
        }

        private void GenerarHTML(DateTime fecha, DateTime fechaGeneracion, string autor, string nombreUsuario, int balanceId, DataTable dt, DataTable estadisticas)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='reporte-container'>");

            // Encabezado del reporte visual con más información
            sb.Append("<div class='reporte-header'>");
            sb.Append("<h2>Balance General</h2>");
            sb.Append($"<p><strong>Fecha del Reporte:</strong> {fecha:dd/MM/yyyy}</p>");
            sb.Append($"<p><strong>Generado el:</strong> {fechaGeneracion:dd/MM/yyyy HH:mm}</p>");
            sb.Append($"<p><strong>Generado por:</strong> {autor} ({nombreUsuario})</p>");
            sb.Append($"<p><strong>ID del Reporte:</strong> #{balanceId}</p>");
            sb.Append("</div>");

            // Sección de Estadísticas
            sb.Append("<div class='estadisticas-section'>");
            sb.Append("<h3 class='seccion-titulo'>Estad&iacute;sticas del Balance</h3>");
            sb.Append("<div class='estadisticas-grid'>");
            
            int totalCuentas = dt.Rows.Count;
            decimal totalActivo = 0, totalPasivo = 0, totalPatrimonio = 0;
            
            foreach (DataRow row in estadisticas.Rows)
            {
                string categoria = row["CategoriaCuenta"].ToString();
                int cantidad = (int)row["CantidadCuentas"];
                decimal total = (decimal)row["TotalCategoria"];
                
                sb.Append($"<div class='estadistica-card'>");
                sb.Append($"<div class='estadistica-label'>{categoria}s</div>");
                sb.Append($"<div class='estadistica-cantidad'>{cantidad} cuenta(s)</div>");
                sb.Append($"<div class='estadistica-monto'>{total:N2}</div>");
                sb.Append("</div>");
                
                if (categoria == "Activo") totalActivo = total;
                else if (categoria == "Pasivo") totalPasivo = total;
                else if (categoria == "Patrimonio") totalPatrimonio = total;
            }
            
            sb.Append($"<div class='estadistica-card total'>");
            sb.Append($"<div class='estadistica-label'>Total Cuentas</div>");
            sb.Append($"<div class='estadistica-cantidad'>{totalCuentas}</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");

            // Función auxiliar local para generar secciones
            decimal calcTotalActivo = 0, calcTotalPasivo = 0, calcTotalPatrimonio = 0;
            
            void GenerarSeccion(string categoria, ref decimal total)
            {
                DataRow[] filas = dt.Select($"CategoriaCuenta = '{categoria}'");
                if (filas.Length > 0)
                {
                    sb.Append($"<div class='categoria-titulo'>{categoria}s ({filas.Length} cuenta(s))</div>");
                    // Calcular total primero para porcentajes
                    decimal totalCategoria = filas.Cast<DataRow>().Sum(r => (decimal)r["Monto"]);
                    
                    // Agrupar cuentas de Patrimonio para mostrar desglose más detallado
                    if (categoria == "Patrimonio")
                    {
                        // Separar cuentas base de patrimonio de las calculadas
                        var cuentasBase = filas.Where(r => r["NombreCuenta"].ToString() != "Utilidad del Ejercicio" && 
                                                          r["NombreCuenta"].ToString() != "Resultados Acumulados").ToArray();
                        var utilidadEjercicio = filas.FirstOrDefault(r => r["NombreCuenta"].ToString() == "Utilidad del Ejercicio");
                        var resultadosAcumulados = filas.FirstOrDefault(r => r["NombreCuenta"].ToString() == "Resultados Acumulados");
                        
                        // Mostrar cuentas base del patrimonio
                        if (cuentasBase.Length > 0)
                        {
                            sb.Append("<div style='margin-left: 1rem; padding-left: 1rem; border-left: 2px solid #2c5aa0;'>");
                            sb.Append("<div style='font-weight: 600; color: #475569; margin-bottom: 0.5rem; font-size: 0.9rem;'>Cuentas de Patrimonio:</div>");
                            foreach (DataRow row in cuentasBase)
                            {
                                decimal monto = (decimal)row["Monto"];
                                decimal porcentaje = totalCategoria > 0 ? (monto / totalCategoria) * 100 : 0;
                                string codigo = row["CodigoCuenta"] != DBNull.Value ? row["CodigoCuenta"].ToString() : "";
                                string codigoDisplay = !string.IsNullOrEmpty(codigo) ? $"({codigo}) " : "";
                                sb.Append($"<div class='fila-detalle'>");
                                sb.Append($"<span>{codigoDisplay}{row["NombreCuenta"]}</span>");
                                sb.Append($"<span class='monto-valor'>{monto:N2}</span>");
                                if (porcentaje > 0)
                                    sb.Append($"<span class='porcentaje'>({porcentaje:F1}%)</span>");
                                sb.Append("</div>");
                                total += monto;
                            }
                            sb.Append("</div>");
                        }
                        
                        // Mostrar Utilidad del Ejercicio con desglose
                        if (utilidadEjercicio != null)
                        {
                            decimal montoUtilidad = (decimal)utilidadEjercicio["Monto"];
                            decimal porcentaje = totalCategoria > 0 ? (montoUtilidad / totalCategoria) * 100 : 0;
                            sb.Append("<div style='margin-left: 1rem; padding-left: 1rem; border-left: 2px solid #10b981; margin-top: 0.5rem;'>");
                            sb.Append("<div style='font-weight: 600; color: #059669; margin-bottom: 0.5rem; font-size: 0.9rem;'>Resultado del Ejercicio:</div>");
                            sb.Append($"<div class='fila-detalle' style='background-color: #f0fdf4;'>");
                            sb.Append($"<span><svg class='icon-finance-inline' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='#059669' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M20 6L9 17l-5-5'/></svg>Utilidad del Ejercicio</span>");
                            sb.Append($"<span class='monto-valor' style='color: #059669;'>{montoUtilidad:N2}</span>");
                            if (porcentaje > 0)
                                sb.Append($"<span class='porcentaje'>({porcentaje:F1}%)</span>");
                            sb.Append("</div>");
                            sb.Append("</div>");
                            total += montoUtilidad;
                        }
                        
                        // Mostrar Resultados Acumulados
                        if (resultadosAcumulados != null)
                        {
                            decimal montoAcum = (decimal)resultadosAcumulados["Monto"];
                            decimal porcentaje = totalCategoria > 0 ? (montoAcum / totalCategoria) * 100 : 0;
                            sb.Append("<div style='margin-left: 1rem; padding-left: 1rem; border-left: 2px solid #6366f1; margin-top: 0.5rem;'>");
                            sb.Append("<div style='font-weight: 600; color: #4f46e5; margin-bottom: 0.5rem; font-size: 0.9rem;'>Resultados Acumulados:</div>");
                            sb.Append($"<div class='fila-detalle' style='background-color: #eef2ff;'>");
                            sb.Append($"<span><svg class='icon-finance-inline' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='#6366f1' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><line x1='18' y1='20' x2='18' y2='10'/><line x1='12' y1='20' x2='12' y2='4'/><line x1='6' y1='20' x2='6' y2='14'/></svg>Resultados Acumulados (años anteriores)</span>");
                            sb.Append($"<span class='monto-valor' style='color: #4f46e5;'>{montoAcum:N2}</span>");
                            if (porcentaje > 0)
                                sb.Append($"<span class='porcentaje'>({porcentaje:F1}%)</span>");
                            sb.Append("</div>");
                            sb.Append("</div>");
                            total += montoAcum;
                        }
                    }
                    else
                    {
                        // Para Activos y Pasivos, mostrar con códigos si están disponibles
                        foreach (DataRow row in filas)
                        {
                            decimal monto = (decimal)row["Monto"];
                            decimal porcentaje = totalCategoria > 0 ? (monto / totalCategoria) * 100 : 0;
                            string codigo = row["CodigoCuenta"] != DBNull.Value ? row["CodigoCuenta"].ToString() : "";
                            string codigoDisplay = !string.IsNullOrEmpty(codigo) ? $"({codigo}) " : "";
                            sb.Append($"<div class='fila-detalle'>");
                            sb.Append($"<span>{codigoDisplay}{row["NombreCuenta"]}</span>");
                            sb.Append($"<span class='monto-valor'>{monto:N2}</span>");
                            if (porcentaje > 0)
                                sb.Append($"<span class='porcentaje'>({porcentaje:F1}%)</span>");
                            sb.Append("</div>");
                            total += monto;
                        }
                    }
                    sb.Append($"<div class='fila-total'><span>Total {categoria}s</span><span>{total:N2}</span></div>");
                }
            }

            // Calcular totales primero
            foreach (DataRow row in dt.Rows)
            {
                string cat = row["CategoriaCuenta"].ToString();
                decimal monto = (decimal)row["Monto"];
                if (cat == "Activo") calcTotalActivo += monto;
                else if (cat == "Pasivo") calcTotalPasivo += monto;
                else if (cat == "Patrimonio") calcTotalPatrimonio += monto;
            }

            GenerarSeccion("Activo", ref calcTotalActivo);
            GenerarSeccion("Pasivo", ref calcTotalPasivo);
            GenerarSeccion("Patrimonio", ref calcTotalPatrimonio);

            // Análisis de Ratios
            sb.Append("<div class='ratios-section'>");
            sb.Append("<h3 class='seccion-titulo'>An&aacute;lisis Financiero</h3>");
            
            decimal totalPP = calcTotalPasivo + calcTotalPatrimonio;
            
            // CORREGIDO: Cálculos de ratios financieros mejorados
            decimal ratioEndeudamiento = calcTotalActivo > 0 ? (calcTotalPasivo / calcTotalActivo) * 100 : 0;
            decimal ratioPatrimonio = calcTotalActivo > 0 ? (calcTotalPatrimonio / calcTotalActivo) * 100 : 0;
            decimal ratioAutonomia = calcTotalPasivo > 0 ? (calcTotalPatrimonio / calcTotalPasivo) * 100 : 0;
            decimal ratioLiquidez = calcTotalPasivo > 0 ? (calcTotalActivo / calcTotalPasivo) : 0;
            
            sb.Append($"<div class='ratio-item'><span>Ratio de Endeudamiento:</span><span>{ratioEndeudamiento:F2}%</span></div>");
            sb.Append($"<div class='ratio-item'><span>Ratio de Patrimonio:</span><span>{ratioPatrimonio:F2}%</span></div>");
            sb.Append($"<div class='ratio-item'><span>Ratio de Autonomía Financiera:</span><span>{ratioAutonomia:F2}%</span></div>");
            sb.Append($"<div class='ratio-item'><span>Ratio de Solvencia:</span><span>{ratioLiquidez:F2}</span></div>");
            sb.Append("</div>");

            // Total Pasivo + Patrimonio
            sb.Append($"<div class='fila-total' style='border-top: 3px solid #2c5aa0; margin-top: 20px; padding-top: 1rem;'><span>TOTAL PASIVO + PATRIMONIO</span><span>{totalPP:N2}</span></div>");

            // Verificación
            decimal diferencia = Math.Abs(calcTotalActivo - totalPP);
            if (diferencia < 0.01m)
                sb.Append("<div class='balance-status success'><strong><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='#059669' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M20 6L9 17l-5-5'/></svg>BALANCE CUADRADO</strong><br/>Los activos igualan a pasivos más patrimonio.</div>");
            else
                sb.Append($"<div class='balance-status error'><strong><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='#dc2626' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><line x1='18' y1='6' x2='6' y2='18'/><line x1='6' y1='6' x2='18' y2='18'/></svg>BALANCE DESCUADRADO</strong><br/>Diferencia: {diferencia:N2}</div>");

            sb.Append("</div>");
            ltlReporte.Text = sb.ToString();
        }
    }
}