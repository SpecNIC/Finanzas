using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;

namespace Software
{
    public partial class EstadoResultado : System.Web.UI.Page
    {
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
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarEstadosResultados();
            }
        }

        private void CargarEstadosResultados()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string query = @"SELECT 
                                        E.EstadoResultadoID, 
                                        E.Periodo,
                                        U.NombreCompleto AS Autor, 
                                        E.FechaGeneracion 
                                     FROM 
                                        EstadosResultados E
                                     JOIN 
                                        Usuarios U ON E.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        E.FechaGeneracion DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEstadosResultados.DataSource = dt;
                    gvEstadosResultados.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar el historial: " + ex.Message, true);
                }
            }
        }

        protected void gvEstadosResultados_RowDataBound(object sender, GridViewRowEventArgs e)
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
                    string queryDelete = "DELETE FROM EstadosResultados WHERE EstadoResultadoID = @EstadoID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@EstadoID", estadoID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Estado de Resultados eliminado correctamente.", false);
                            CargarEstadosResultados();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el estado de resultados.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el estado de resultados: " + ex.Message, true);
                }
            }
        }

        protected void gvEstadosResultados_SelectedIndexChanged(object sender, EventArgs e)
        {
            int estadoID = (int)gvEstadosResultados.SelectedDataKey.Value;
            Session["EstadoResultadoSeleccionadoID"] = estadoID;

            btnExportarExcel.Enabled = true;
            btnVerDetalle.Enabled = true;

            MostrarMensaje("Estado de Resultados ID " + estadoID + " seleccionado.", false);
        }

        protected void btnNuevoEstado_Click(object sender, EventArgs e)
        {
            Response.Redirect("CalcularEstadoResultado.aspx");
        }

        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (Session["EstadoResultadoSeleccionadoID"] != null)
            {
                int estadoID = (int)Session["EstadoResultadoSeleccionadoID"];
                Response.Redirect("VerEstadoResultado.aspx?ID=" + estadoID);
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (Session["EstadoResultadoSeleccionadoID"] != null)
            {
                int estadoID = (int)Session["EstadoResultadoSeleccionadoID"];
                ExportarEstadoResultadoExcel(estadoID);
            }
            else
            {
                MostrarMensaje("Error: No hay ningún estado de resultados seleccionado.", true);
            }
        }

        private void ExportarEstadoResultadoExcel(int estadoID)
        {
            string periodo = "";
            DataTable dtDetalle = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string queryHeader = "SELECT Periodo FROM EstadosResultados WHERE EstadoResultadoID = @EstadoID";
                using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                {
                    cmd.Parameters.AddWithValue("@EstadoID", estadoID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            periodo = reader["Periodo"].ToString();
                        }
                    }
                }

                string queryDetalle = @"
                    SELECT IngresosVentas, CostoBienesVendidos, GastosOperativos, GastosIntereses, 
                           TasaImpuestos, DividendosEfectivo, UtilidadBruta, UtilidadOperativa, 
                           UtilidadNetaAntesImpuestos, Impuestos, UtilidadNetaDespuesImpuestos, GananciasRetenidas
                    FROM DetalleEstadoResultado 
                    WHERE EstadoResultadoID = @EstadoID";

                using (SqlCommand cmd = new SqlCommand(queryDetalle, con))
                {
                    cmd.Parameters.AddWithValue("@EstadoID", estadoID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtDetalle);
                }
            }

            if (dtDetalle.Rows.Count == 0)
            {
                MostrarMensaje("No se encontraron datos para exportar.", true);
                return;
            }

            DataRow row = dtDetalle.Rows[0];
            decimal ingresosVentas = Convert.ToDecimal(row["IngresosVentas"]);
            decimal costoBienesVendidos = Convert.ToDecimal(row["CostoBienesVendidos"]);
            decimal gastosOperativos = Convert.ToDecimal(row["GastosOperativos"]);
            decimal gastosIntereses = Convert.ToDecimal(row["GastosIntereses"]);
            decimal tasaImpuestos = Convert.ToDecimal(row["TasaImpuestos"]);
            decimal dividendosEfectivo = Convert.ToDecimal(row["DividendosEfectivo"]);
            decimal utilidadBruta = Convert.ToDecimal(row["UtilidadBruta"]);
            decimal utilidadOperativa = Convert.ToDecimal(row["UtilidadOperativa"]);
            decimal utilidadNetaAntesImpuestos = Convert.ToDecimal(row["UtilidadNetaAntesImpuestos"]);
            decimal impuestos = Convert.ToDecimal(row["Impuestos"]);
            decimal utilidadNetaDespuesImpuestos = Convert.ToDecimal(row["UtilidadNetaDespuesImpuestos"]);
            decimal gananciasRetenidas = Convert.ToDecimal(row["GananciasRetenidas"]);

            StringBuilder sb = new StringBuilder();
            
            // Agregar declaración HTML con charset UTF-8
            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            sb.Append("</head>");
            sb.Append("<body>");
            
            sb.Append("<table border='1' style='font-family:Arial; font-size:12px;'>");

            sb.Append("<tr><td colspan='2' style='background-color:#0056b3; color:white; font-size:16px; font-weight:bold; text-align:center;'>ESTADO DE RESULTADOS</td></tr>");
            sb.AppendFormat("<tr><td colspan='2' style='text-align:center; font-weight:bold;'>Per&iacute;odo: {0}</td></tr>", periodo);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.AppendFormat("<tr><td>Ingresos por ventas</td><td style='text-align:right;'>{0:N2}</td></tr>", ingresosVentas);
            sb.AppendFormat("<tr><td>Menos: Costo de los bienes vendidos</td><td style='text-align:right;'>{0:N2}</td></tr>", costoBienesVendidos);
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>Utilidad bruta</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", utilidadBruta);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.AppendFormat("<tr><td>Menos: Gastos operativos</td><td style='text-align:right;'>{0:N2}</td></tr>", gastosOperativos);
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>Utilidad operativa</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", utilidadOperativa);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.AppendFormat("<tr><td>Menos: Gastos por intereses</td><td style='text-align:right;'>{0:N2}</td></tr>", gastosIntereses);
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>Utilidad neta antes de impuestos</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", utilidadNetaAntesImpuestos);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.AppendFormat("<tr><td>Menos: Impuestos (tasa = {0}%)</td><td style='text-align:right;'>{1:N2}</td></tr>", tasaImpuestos, impuestos);
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#e6e6e6;'>Utilidad neta despu&eacute;s de impuestos</td><td style='font-weight:bold; text-align:right; background-color:#e6e6e6;'>{0:N2}</td></tr>", utilidadNetaDespuesImpuestos);
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.AppendFormat("<tr><td>Menos: Dividendos en efectivo</td><td style='text-align:right;'>{0:N2}</td></tr>", dividendosEfectivo);
            sb.AppendFormat("<tr><td style='font-weight:bold; background-color:#d4edda; color:#155724; border-top:3px double black;'>A ganancias retenidas</td><td style='font-weight:bold; text-align:right; background-color:#d4edda; color:#155724; border-top:3px double black; font-size:14px;'>{0:N2}</td></tr>", gananciasRetenidas);

            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=EstadoResultado_" + periodo.Replace(" ", "_") + ".xls");
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}

