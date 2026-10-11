using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;

namespace Software
{
    public partial class VerEstadoOrigen : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null) Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null && int.TryParse(Request.QueryString["ID"], out int estadoID))
                {
                    CargarDetalleEstadoOrigen(estadoID);
                }
                else
                {
                    lblMensaje.Text = "ID de reporte inválido.";
                    lblMensaje.Visible = true;
                }
            }
        }

        private void CargarDetalleEstadoOrigen(int estadoFlujoID)
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
                                            E.FechaInicio, 
                                            E.FechaFin,
                                            E.FechaGeneracion,
                                            U.NombreCompleto AS Autor,
                                            U.NombreUsuario,
                                            E.EstadoFlujoID,
                                            E.UsuarioID
                                           FROM EstadosFlujo E
                                           INNER JOIN Usuarios U ON E.UsuarioID = U.UsuarioID
                                           WHERE E.EstadoFlujoID = @ID";
                    
                    DateTime fechaInicio = DateTime.MinValue;
                    DateTime fechaFin = DateTime.MinValue;
                    DateTime fechaGeneracion = DateTime.MinValue;
                    string autor = "";
                    string nombreUsuario = "";
                    int estadoId = 0;
                    int estadoUsuarioID = 0;

                    using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", estadoFlujoID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                estadoUsuarioID = (int)r["UsuarioID"];
                                
                                // Verificar permisos
                                if (!esAdmin && estadoUsuarioID != usuarioID)
                                {
                                    lblMensaje.Text = "No tienes permiso para ver este reporte.";
                                    lblMensaje.CssClass = "feedback-message error";
                                    lblMensaje.Visible = true;
                                    return;
                                }
                                
                                fechaInicio = (DateTime)r["FechaInicio"];
                                fechaFin = (DateTime)r["FechaFin"];
                                fechaGeneracion = (DateTime)r["FechaGeneracion"];
                                autor = r["Autor"].ToString();
                                nombreUsuario = r["NombreUsuario"].ToString();
                                estadoId = (int)r["EstadoFlujoID"];
                            }
                            else 
                            { 
                                lblMensaje.Text = "Reporte no encontrado."; 
                                lblMensaje.Visible = true; 
                                return; 
                            }
                        }
                    }

                    // 2. Obtener las líneas del Detalle
                    string queryDetail = @"SELECT CategoriaFlujo, NombreLinea, Monto 
                                          FROM DetalleEstadoFlujo 
                                          WHERE EstadoFlujoID = @ID 
                                          ORDER BY 
                                            CASE CategoriaFlujo 
                                                WHEN 'Origen' THEN 1 
                                                WHEN 'Aplicacion' THEN 2 
                                                ELSE 3 END,
                                            NombreLinea";
                    SqlDataAdapter da = new SqlDataAdapter(queryDetail, con);
                    da.SelectCommand.Parameters.AddWithValue("@ID", estadoFlujoID);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // 3. Construir el HTML
                    GenerarHTML(fechaInicio, fechaFin, fechaGeneracion, autor, nombreUsuario, estadoId, dt);
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
                    lblMensaje.Visible = true;
                }
            }
        }

        private void GenerarHTML(DateTime fechaInicio, DateTime fechaFin, DateTime fechaGeneracion, string autor, string nombreUsuario, int estadoId, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='reporte-container'>");

            // Encabezado del reporte
            sb.Append("<div class='reporte-header'>");
            sb.Append("<h2>Estado de Origen y Aplicaci&oacute;n</h2>");
            sb.Append($"<p><strong>Período:</strong> Del {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}</p>");
            sb.Append($"<p><strong>Generado el:</strong> {fechaGeneracion:dd/MM/yyyy HH:mm}</p>");
            sb.Append($"<p><strong>Generado por:</strong> {autor} ({nombreUsuario})</p>");
            sb.Append($"<p><strong>ID del Reporte:</strong> #{estadoId}</p>");
            sb.Append("</div>");

            // Sección de Estadísticas
            sb.Append("<div class='estadisticas-section'>");
            sb.Append("<h3 class='seccion-titulo'>Resumen del Estado</h3>");
            sb.Append("<div class='estadisticas-grid'>");
            
            int totalLineas = dt.Rows.Count;
            decimal totalOrigen = 0, totalAplicacion = 0;
            
            DataRow[] filasOrigen = dt.Select("CategoriaFlujo = 'Origen'");
            DataRow[] filasAplicacion = dt.Select("CategoriaFlujo = 'Aplicacion'");
            
            totalOrigen = filasOrigen.Cast<DataRow>().Sum(r => (decimal)r["Monto"]);
            totalAplicacion = filasAplicacion.Cast<DataRow>().Sum(r => (decimal)r["Monto"]);
            
            sb.Append($"<div class='estadistica-card'>");
            sb.Append($"<div class='estadistica-label'>Total Origen</div>");
            sb.Append($"<div class='estadistica-monto'>{totalOrigen:N2}</div>");
            sb.Append("</div>");
            
            sb.Append($"<div class='estadistica-card'>");
            sb.Append($"<div class='estadistica-label'>Total Aplicaci&oacute;n</div>");
            sb.Append($"<div class='estadistica-monto'>{totalAplicacion:N2}</div>");
            sb.Append("</div>");
            
            decimal variacionNeta = totalOrigen - totalAplicacion;
            string claseVariacion = variacionNeta >= 0 ? "estadistica-card total" : "estadistica-card";
            sb.Append($"<div class='{claseVariacion}'>");
            sb.Append($"<div class='estadistica-label'>Variación Neta</div>");
            sb.Append($"<div class='estadistica-monto'>{variacionNeta:N2}</div>");
            sb.Append("</div>");
            
            sb.Append("</div>");
            sb.Append("</div>");

            // Sección Origen
            if (filasOrigen.Length > 0)
            {
                string textoLineas = filasOrigen.Length == 1 ? "1 línea" : $"{filasOrigen.Length} líneas";
                sb.Append($"<div class='categoria-titulo'>Origen de Fondos ({textoLineas})</div>");
                foreach (DataRow row in filasOrigen)
                {
                    decimal monto = (decimal)row["Monto"];
                    sb.Append($"<div class='fila-detalle'>");
                    sb.Append($"<span>{row["NombreLinea"]}</span>");
                    sb.Append($"<span class='monto-valor'>{monto:N2}</span>");
                    sb.Append("</div>");
                }
                sb.Append($"<div class='fila-total'><span>Total Origen</span><span>{totalOrigen:N2}</span></div>");
            }

            // Sección Aplicación
            if (filasAplicacion.Length > 0)
            {
                string textoLineas = filasAplicacion.Length == 1 ? "1 línea" : $"{filasAplicacion.Length} líneas";
                sb.Append($"<div class='categoria-titulo' style='margin-top: 2rem;'>Aplicaci&oacute;n de Fondos ({textoLineas})</div>");
                foreach (DataRow row in filasAplicacion)
                {
                    decimal monto = (decimal)row["Monto"];
                    sb.Append($"<div class='fila-detalle'>");
                    sb.Append($"<span>{row["NombreLinea"]}</span>");
                    sb.Append($"<span class='monto-valor'>{monto:N2}</span>");
                    sb.Append("</div>");
                }
                sb.Append($"<div class='fila-total'><span>Total Aplicaci&oacute;n</span><span>{totalAplicacion:N2}</span></div>");
            }

            // Variación Neta Final
            sb.Append($"<div class='fila-total' style='border-top: 3px solid #2c5aa0; margin-top: 20px; padding-top: 1rem;'><span>VARIACIÓN NETA EN EFECTIVO</span><span>{variacionNeta:N2}</span></div>");

            // Verificación
            if (variacionNeta > 0)
                sb.Append("<div class='balance-status success'><strong><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='#059669' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M20 6L9 17l-5-5'/></svg>Flujo positivo</strong><br/>El efectivo aumentó en el período.</div>");
            else if (variacionNeta < 0)
                sb.Append("<div class='balance-status' style='background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%); color: #991b1b; border: 2px solid #ef4444;'><strong><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z'/><line x1='12' y1='9' x2='12' y2='13'/><line x1='12' y1='17' x2='12.01' y2='17'/></svg>Flujo negativo</strong><br/>El efectivo disminuyó en el período.</div>");
            else
                sb.Append("<div class='balance-status success' style='background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%); color: #0369a1; border: 2px solid #0ea5e9;'><strong><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><line x1='12' y1='3' x2='12' y2='21'/><path d='M17 12H7M17 12l-4-4M17 12l-4 4M7 12l4-4M7 12l4 4'/></svg>Flujo equilibrado</strong><br/>No hubo variación en el efectivo durante el período.</div>");

            sb.Append("</div>");
            ltlReporte.Text = sb.ToString();
        }
    }
}

