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
using System.Web.UI.HtmlControls; // Para controles HTML

namespace Software
{
    public partial class BalanceGeneral : System.Web.UI.Page
    {
        // Leer la cadena de conexión desde Web.config
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
                // Si es la primera vez que carga la página, llena la tabla
                CargarBalances();
            }
        }

        /// <summary>
        /// Se conecta a la BD y carga el historial de balances en el GridView.
        /// </summary>
        private void CargarBalances()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    // Consulta que une BalancesGenerales con Usuarios para obtener el nombre del autor
                    string query = @"SELECT 
                                        B.BalanceID, 
                                        B.FechaReporte, 
                                        U.NombreCompleto AS Autor, 
                                        B.FechaGeneracion 
                                     FROM 
                                        BalancesGenerales B
                                     JOIN 
                                        Usuarios U ON B.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR B.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        B.FechaReporte DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvBalances.DataSource = dt;
                    gvBalances.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar el historial: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Verifica si el usuario actual es administrador.
        /// </summary>
        private bool EsAdministrador()
        {
            return Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
        }

        /// <summary>
        /// Maneja el evento RowDataBound para configurar controles de edición y permisos.
        /// </summary>
        protected void gvBalances_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool esAdmin = EsAdministrador();
                int balanceID = (int)gvBalances.DataKeys[e.Row.RowIndex].Value;
                bool editMode = ViewState["EditMode_" + balanceID] != null && (bool)ViewState["EditMode_" + balanceID];
                
                Label lblFecha = e.Row.FindControl("lblFechaReporte") as Label;
                TextBox txtFecha = e.Row.FindControl("txtFechaReporte") as TextBox;
                LinkButton btnEditar = e.Row.FindControl("btnEditarFecha") as LinkButton;
                LinkButton btnGuardar = e.Row.FindControl("btnGuardarFecha") as LinkButton;
                LinkButton btnCancelar = e.Row.FindControl("btnCancelarFecha") as LinkButton;
                LinkButton btnEliminar = e.Row.FindControl("btnEliminar") as LinkButton;
                
                if (lblFecha != null) lblFecha.Visible = !editMode;
                if (txtFecha != null) txtFecha.Visible = editMode;
                
                // Solo administradores pueden editar y eliminar
                if (btnEditar != null) btnEditar.Visible = !editMode && esAdmin;
                if (btnGuardar != null) btnGuardar.Visible = editMode && esAdmin;
                if (btnCancelar != null) btnCancelar.Visible = editMode && esAdmin;
                if (btnEliminar != null) btnEliminar.Visible = esAdmin;
            }
        }

        /// <summary>
        /// Maneja la edición de fecha del reporte.
        /// </summary>
        protected void btnEditarFecha_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int balanceID = Convert.ToInt32(e.CommandArgument);
            ViewState["EditMode_" + balanceID] = true;
            CargarBalances();
        }

        /// <summary>
        /// Guarda la fecha modificada del reporte.
        /// </summary>
        protected void btnGuardarFecha_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int balanceID = Convert.ToInt32(e.CommandArgument);
            
            // Obtener el TextBox con la nueva fecha
            GridViewRow row = null;
            foreach (GridViewRow r in gvBalances.Rows)
            {
                LinkButton btn = r.FindControl("btnGuardarFecha") as LinkButton;
                if (btn != null && btn.CommandArgument == balanceID.ToString())
                {
                    row = r;
                    break;
                }
            }

            if (row != null)
            {
                TextBox txtFecha = row.FindControl("txtFechaReporte") as TextBox;
                if (txtFecha != null && !string.IsNullOrWhiteSpace(txtFecha.Text))
                {
                    if (DateTime.TryParse(txtFecha.Text, out DateTime nuevaFecha))
                    {
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            try
                            {
                                con.Open();
                                string query = "UPDATE BalancesGenerales SET FechaReporte = @FechaReporte WHERE BalanceID = @BalanceID";
                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    cmd.Parameters.AddWithValue("@FechaReporte", nuevaFecha);
                                    cmd.Parameters.AddWithValue("@BalanceID", balanceID);
                                    cmd.ExecuteNonQuery();
                                }
                                ViewState["EditMode_" + balanceID] = false;
                                MostrarMensaje("Fecha del reporte actualizada correctamente.", false);
                                CargarBalances();
                            }
                            catch (Exception ex)
                            {
                                MostrarMensaje("Error al actualizar la fecha: " + ex.Message, true);
                            }
                        }
                    }
                    else
                    {
                        MostrarMensaje("La fecha ingresada no es válida.", true);
                    }
                }
            }
        }

        /// <summary>
        /// Cancela la edición de fecha del reporte.
        /// </summary>
        protected void btnCancelarFecha_Command(object sender, CommandEventArgs e)
        {
            int balanceID = Convert.ToInt32(e.CommandArgument);
            ViewState["EditMode_" + balanceID] = false;
            CargarBalances();
        }

        /// <summary>
        /// Elimina un balance general (solo administradores).
        /// </summary>
        protected void btnEliminar_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int balanceID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Eliminar primero los detalles (cascada) y luego el balance
                    string queryDelete = "DELETE FROM BalancesGenerales WHERE BalanceID = @BalanceID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@BalanceID", balanceID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Balance eliminado correctamente.", false);
                            CargarBalances();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el balance.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el balance: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Se dispara cuando el usuario hace clic en "Seleccionar" en una fila del GridView.
        /// </summary>
        protected void gvBalances_SelectedIndexChanged(object sender, EventArgs e)
        {
            int balanceID = (int)gvBalances.SelectedDataKey.Value;
            Session["BalanceSeleccionadoID"] = balanceID;

            btnExportarExcel.Enabled = true;
            btnVerDetalle.Enabled = true; // <--- HABILITAR EL BOTÓN NUEVO

            MostrarMensaje("Balance ID " + balanceID + " seleccionado.", false);
        }

        // 2. Agrega el evento Click del nuevo botón
        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (Session["BalanceSeleccionadoID"] != null)
            {
                int balanceID = (int)Session["BalanceSeleccionadoID"];
                // Redirige a la nueva página de detalle, pasando el ID en la URL
                Response.Redirect("VerBalance.aspx?ID=" + balanceID);
            }
        }

        /// <summary>
        /// Redirige al usuario a la página de cálculo.
        /// </summary>
        protected void btnNuevoBalance_Click(object sender, EventArgs e)
        {
            // Redirigimos a una nueva página que construiremos después
            Response.Redirect("CalcularBalance.aspx");
        }

        /// <summary>
        /// (Lógica pendiente) Se encarga de exportar el balance seleccionado.
        /// </summary>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (Session["BalanceSeleccionadoID"] != null)
            {
                int balanceID = (int)Session["BalanceSeleccionadoID"];
                // ¡Llamamos al método de exportación!
                ExportarBalanceExcel(balanceID);
            }
            else
            {
                MostrarMensaje("Error: No hay ningún balance seleccionado.", true);
            }
        }

        // --- MÉTODO NUEVO PARA EXPORTAR ---
        private void ExportarBalanceExcel(int balanceID)
        {
            DateTime fechaReporte = DateTime.Now;
            DataTable dtDetalle = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // A. Obtener Fecha del reporte
                string queryHeader = "SELECT FechaReporte FROM BalancesGenerales WHERE BalanceID = @BalanceID";
                using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                {
                    cmd.Parameters.AddWithValue("@BalanceID", balanceID);
                    object result = cmd.ExecuteScalar();
                    if (result != null) fechaReporte = Convert.ToDateTime(result);
                }

                // B. Obtener el Detalle (Reutilizamos la estructura que ya tienes en BD)
                string queryDetalle = @"
                SELECT CategoriaCuenta, NombreCuenta, Monto 
                FROM DetalleBalance 
                WHERE BalanceID = @BalanceID
                ORDER BY 
                    CASE CategoriaCuenta 
                        WHEN 'Activo' THEN 1 
                        WHEN 'Pasivo' THEN 2 
                        WHEN 'Patrimonio' THEN 3 
                        ELSE 4 END, 
                    NombreCuenta";

                using (SqlCommand cmd = new SqlCommand(queryDetalle, con))
                {
                    cmd.Parameters.AddWithValue("@BalanceID", balanceID);
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

            // Inicio de Tabla con estilos simples que Excel entiende
            sb.Append("<table border='1' style='font-family:Arial; font-size:12px;'>");

            // Título
            sb.Append("<tr><td colspan='2' style='background-color:#0056b3; color:white; font-size:16px; font-weight:bold; text-align:center;'>BALANCE GENERAL</td></tr>");
            sb.AppendFormat("<tr><td colspan='2' style='text-align:center; font-weight:bold;'>Al: {0:dd/MM/yyyy}</td></tr>", fechaReporte);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>"); // Espacio vacío

            // Variables para totales
            decimal totalActivo = 0, totalPasivo = 0, totalPatrimonio = 0;

            // --- SECCIÓN ACTIVOS ---
            sb.Append("<tr><td colspan='2' style='background-color:#f2f2f2; font-weight:bold;'>ACTIVOS</td></tr>");
            foreach (DataRow row in dtDetalle.Select("CategoriaCuenta = 'Activo'"))
            {
                decimal monto = Convert.ToDecimal(row["Monto"]);
                totalActivo += monto;
                string nombreCuenta = ConvertirAEntidadesHTML(row["NombreCuenta"].ToString());
                sb.AppendFormat("<tr><td>{0}</td><td style='text-align:right;'>{1:N2}</td></tr>", nombreCuenta, monto);
            }
            sb.AppendFormat("<tr><td style='font-weight:bold;'>TOTAL ACTIVOS</td><td style='font-weight:bold; text-align:right; border-top:2px solid black;'>{0:N2}</td></tr>", totalActivo);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            // --- SECCIÓN PASIVOS ---
            sb.Append("<tr><td colspan='2' style='background-color:#f2f2f2; font-weight:bold;'>PASIVOS</td></tr>");
            foreach (DataRow row in dtDetalle.Select("CategoriaCuenta = 'Pasivo'"))
            {
                decimal monto = Convert.ToDecimal(row["Monto"]);
                totalPasivo += monto;
                string nombreCuenta = ConvertirAEntidadesHTML(row["NombreCuenta"].ToString());
                sb.AppendFormat("<tr><td>{0}</td><td style='text-align:right;'>{1:N2}</td></tr>", nombreCuenta, monto);
            }
            sb.AppendFormat("<tr><td style='font-weight:bold;'>TOTAL PASIVOS</td><td style='font-weight:bold; text-align:right; border-top:2px solid black;'>{0:N2}</td></tr>", totalPasivo);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            // --- SECCIÓN PATRIMONIO ---
            sb.Append("<tr><td colspan='2' style='background-color:#f2f2f2; font-weight:bold;'>PATRIMONIO</td></tr>");
            foreach (DataRow row in dtDetalle.Select("CategoriaCuenta = 'Patrimonio'"))
            {
                decimal monto = Convert.ToDecimal(row["Monto"]);
                totalPatrimonio += monto;
                string nombreCuenta = ConvertirAEntidadesHTML(row["NombreCuenta"].ToString());
                sb.AppendFormat("<tr><td>{0}</td><td style='text-align:right;'>{1:N2}</td></tr>", nombreCuenta, monto);
            }
            sb.AppendFormat("<tr><td style='font-weight:bold;'>TOTAL PATRIMONIO</td><td style='font-weight:bold; text-align:right; border-top:2px solid black;'>{0:N2}</td></tr>", totalPatrimonio);

            // Totales finales
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>TOTAL PASIVO + PATRIMONIO</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", totalPasivo + totalPatrimonio);

            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");

            // D. Enviar al navegador
            Response.Clear();
            Response.Buffer = true;
            // Usamos .xls para compatibilidad con HTML.
            Response.AddHeader("content-disposition", "attachment;filename=Balance_" + fechaReporte.ToString("yyyyMMdd") + ".xls");
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