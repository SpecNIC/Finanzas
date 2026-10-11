using System;
using System.Web.UI;
// --- ¡Añadidos 'using' para exportación! ---
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text; // Para StringBuilder
using System.Web.UI.WebControls; // Para GridViewRow
using System.IO; // Para StringWriter
using System.Web; // Para HttpContext

namespace Software
{
    public partial class EstadoOrigenAplicacion : System.Web.UI.Page
    {
        // Leer la cadena de conexión desde Web.config
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Verifica si el usuario actual es administrador.
        /// </summary>
        private bool EsAdministrador()
        {
            return Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- VERIFICACIÓN DE SEGURIDAD ---
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarEstados();
            }
        }

        /// <summary>
        /// Se conecta a la BD y carga el historial de estados de flujo.
        /// </summary>
        private void CargarEstados()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    // Consulta que une EstadosFlujo con Usuarios para obtener el nombre del autor
                    string query = @"SELECT 
                                        E.EstadoFlujoID, 
                                        E.FechaInicio,
                                        E.FechaFin,
                                        U.NombreCompleto AS Autor, 
                                        E.FechaGeneracion 
                                     FROM 
                                        EstadosFlujo E
                                     JOIN 
                                        Usuarios U ON E.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        E.FechaFin DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEstadosOrigen.DataSource = dt;
                    gvEstadosOrigen.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar el historial: " + ex.Message, true);
                }
            }
        }

        protected void gvEstadosOrigen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool esAdmin = EsAdministrador();
                LinkButton btnEliminar = e.Row.FindControl("btnEliminar") as LinkButton;
                
                // Solo administradores pueden eliminar
                if (btnEliminar != null)
                {
                    btnEliminar.Visible = esAdmin;
                }
            }
        }

        protected void btnEliminar_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int estadoID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Eliminar primero los detalles (cascada) y luego el estado
                    string queryDelete = "DELETE FROM EstadosFlujo WHERE EstadoFlujoID = @EstadoID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@EstadoID", estadoID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Estado de Origen y Aplicación eliminado correctamente.", false);
                            CargarEstados();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el estado de origen y aplicación.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el estado de origen y aplicación: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Se dispara cuando el usuario hace clic en "Seleccionar" en una fila.
        /// </summary>
        protected void gvEstadosOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            int estadoID = (int)gvEstadosOrigen.SelectedDataKey.Value;
            Session["EstadoFlujoSeleccionadoID"] = estadoID;
            btnExportarExcel.Enabled = true;
            btnVerDetalle.Enabled = true;
            MostrarMensaje("Reporte ID " + estadoID + " seleccionado.", false);
        }

        /// <summary>
        /// Redirige a la página de detalle del estado de origen y aplicación.
        /// </summary>
        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (Session["EstadoFlujoSeleccionadoID"] != null)
            {
                int estadoID = (int)Session["EstadoFlujoSeleccionadoID"];
                Response.Redirect("VerEstadoOrigen.aspx?ID=" + estadoID);
            }
        }

        /// <summary>
        /// Redirige al usuario a la página de cálculo.
        /// </summary>
        protected void btnNuevoEstado_Click(object sender, EventArgs e)
        {
            Response.Redirect("CalcularEstadoOrigen.aspx");
        }

        /// <summary>
        /// (Lógica pendiente) Se encarga de exportar el estado seleccionado.
        /// </summary>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (Session["EstadoFlujoSeleccionadoID"] != null)
            {
                int estadoID = (int)Session["EstadoFlujoSeleccionadoID"];
                // ¡Llamamos al método de exportación!
                ExportarEstadoFlujoExcel(estadoID);
            }
            else
            {
                MostrarMensaje("Error: No hay ningún reporte seleccionado.", true);
            }
        }

        // --- MÉTODO NUEVO PARA EXPORTAR ---
        private void ExportarEstadoFlujoExcel(int estadoFlujoID)
        {
            DateTime fechaInicio = DateTime.Now;
            DateTime fechaFin = DateTime.Now;
            DataTable dtDetalle = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // A. Obtener Fechas del reporte
                string queryHeader = "SELECT FechaInicio, FechaFin FROM EstadosFlujo WHERE EstadoFlujoID = @EstadoFlujoID";
                using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                {
                    cmd.Parameters.AddWithValue("@EstadoFlujoID", estadoFlujoID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
                            fechaFin = Convert.ToDateTime(reader["FechaFin"]);
                        }
                    }
                }

                // B. Obtener el Detalle del reporte
                string queryDetalle = @"
                    SELECT CategoriaFlujo, NombreLinea, Monto 
                    FROM DetalleEstadoFlujo 
                    WHERE EstadoFlujoID = @EstadoFlujoID
                    ORDER BY 
                        CASE CategoriaFlujo 
                            WHEN 'Actividades de Operación' THEN 1 
                            WHEN 'Actividades de Inversión' THEN 2 
                            WHEN 'Actividades de Financiamiento' THEN 3 
                            ELSE 4 END, 
                        NombreLinea";

                using (SqlCommand cmd = new SqlCommand(queryDetalle, con))
                {
                    cmd.Parameters.AddWithValue("@EstadoFlujoID", estadoFlujoID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtDetalle);
                }
            }

            // C. Construir el HTML para Excel
            StringBuilder sb = new StringBuilder();
            
            // Agregar declaración HTML con charset UTF-8
            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            sb.Append("</head>");
            sb.Append("<body>");

            // Inicio de Tabla con estilos
            sb.Append("<table border='1' style='font-family:Arial; font-size:12px;'>");

            // Título
            sb.Append("<tr><td colspan='2' style='background-color:#0056b3; color:white; font-size:16px; font-weight:bold; text-align:center;'>ESTADO DE ORIGEN Y APLICACI&Oacute;N</td></tr>");
            sb.AppendFormat("<tr><td colspan='2' style='text-align:center; font-weight:bold;'>Del: {0:dd/MM/yyyy} Al: {1:dd/MM/yyyy}</td></tr>", fechaInicio, fechaFin);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>"); // Espacio vacío

            // Obtener categorías únicas para iterar
            DataView view = new DataView(dtDetalle);
            DataTable dtCategorias = view.ToTable(true, "CategoriaFlujo");

            decimal granTotal = 0;

            // Agrupar por Origen y Aplicación
            decimal totalOrigen = 0;
            decimal totalAplicacion = 0;

            // --- SECCIÓN ORIGEN ---
            sb.Append("<tr><td colspan='2' style='background-color:#f2f2f2; font-weight:bold;'>Origen</td></tr>");
            foreach (DataRow row in dtDetalle.Select("CategoriaFlujo = 'Origen'"))
            {
                decimal monto = Convert.ToDecimal(row["Monto"]);
                totalOrigen += monto;
                string nombreLinea = ConvertirAEntidadesHTML(row["NombreLinea"].ToString());
                sb.AppendFormat("<tr><td>{0}</td><td style='text-align:right;'>{1:N2}</td></tr>", nombreLinea, monto);
            }
            sb.AppendFormat("<tr><td style='font-weight:bold;'>Total Origen</td><td style='font-weight:bold; text-align:right; border-top:2px solid black;'>{0:N2}</td></tr>", totalOrigen);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            // --- SECCIÓN APLICACIÓN ---
            sb.Append("<tr><td colspan='2' style='background-color:#f2f2f2; font-weight:bold;'>Aplicaci&oacute;n</td></tr>");
            foreach (DataRow row in dtDetalle.Select("CategoriaFlujo = 'Aplicacion'"))
            {
                decimal monto = Convert.ToDecimal(row["Monto"]);
                totalAplicacion += monto;
                string nombreLinea = ConvertirAEntidadesHTML(row["NombreLinea"].ToString());
                sb.AppendFormat("<tr><td>{0}</td><td style='text-align:right;'>{1:N2}</td></tr>", nombreLinea, monto);
            }
            sb.AppendFormat("<tr><td style='font-weight:bold;'>Total Aplicaci&oacute;n</td><td style='font-weight:bold; text-align:right; border-top:2px solid black;'>{0:N2}</td></tr>", totalAplicacion);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            granTotal = totalOrigen + totalAplicacion;

            // Total Final
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>Variaci&oacute;n Neta en Efectivo</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", granTotal);
            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");

            // D. Enviar al navegador
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=EstadoFlujo_" + fechaFin.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        // --- Función Asistente para convertir caracteres especiales a entidades HTML ---
        private string ConvertirAEntidadesHTML(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto
                .Replace("á", "&aacute;").Replace("Á", "&Aacute;")
                .Replace("é", "&eacute;").Replace("É", "&Eacute;")
                .Replace("í", "&iacute;").Replace("Í", "&Iacute;")
                .Replace("ó", "&oacute;").Replace("Ó", "&Oacute;")
                .Replace("ú", "&uacute;").Replace("Ú", "&Uacute;")
                .Replace("ñ", "&ntilde;").Replace("Ñ", "&Ntilde;");
        }

        // --- Función Asistente para mostrar mensajes ---
        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}