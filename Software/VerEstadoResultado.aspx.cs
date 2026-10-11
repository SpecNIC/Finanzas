using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace Software
{
    public partial class VerEstadoResultado : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null) Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null && int.TryParse(Request.QueryString["ID"], out int estadoID))
                {
                    CargarDetalleEstadoResultado(estadoID);
                }
                else
                {
                    lblMensaje.Text = "ID de reporte inválido.";
                    lblMensaje.Visible = true;
                }
            }
        }

        private void CargarDetalleEstadoResultado(int estadoID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Verificar permisos: solo admin puede ver todos, usuarios regulares solo los suyos
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string queryHeader = "SELECT Periodo, FechaGeneracion, UsuarioID FROM EstadosResultados WHERE EstadoResultadoID = @ID";
                    string periodo = "";
                    DateTime fechaGeneracion = DateTime.Now;
                    int estadoUsuarioID = 0;

                    using (SqlCommand cmd = new SqlCommand(queryHeader, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", estadoID);
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
                                
                                periodo = r["Periodo"].ToString();
                                fechaGeneracion = (DateTime)r["FechaGeneracion"];
                            }
                            else 
                            { 
                                lblMensaje.Text = "Reporte no encontrado."; 
                                lblMensaje.Visible = true; 
                                return; 
                            }
                        }
                    }

                    string queryDetail = @"SELECT IngresosVentas, CostoBienesVendidos, GastosOperativos, GastosIntereses, 
                                         TasaImpuestos, DividendosEfectivo, UtilidadBruta, UtilidadOperativa, 
                                         UtilidadNetaAntesImpuestos, Impuestos, UtilidadNetaDespuesImpuestos, GananciasRetenidas
                                         FROM DetalleEstadoResultado 
                                         WHERE EstadoResultadoID = @ID";
                    
                    SqlDataAdapter da = new SqlDataAdapter(queryDetail, con);
                    da.SelectCommand.Parameters.AddWithValue("@ID", estadoID);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        GenerarHTML(periodo, fechaGeneracion, dt.Rows[0]);
                    }
                    else
                    {
                        lblMensaje.Text = "No se encontraron detalles del reporte.";
                        lblMensaje.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
                    lblMensaje.Visible = true;
                }
            }
        }

        private void GenerarHTML(string periodo, DateTime fechaGeneracion, DataRow row)
        {
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
            sb.Append("<div class='reporte-container'>");

            sb.Append("<div class='reporte-header'>");
            sb.Append("<h2>Estado de Resultados</h2>");
            sb.Append($"<p>Per&iacute;odo: {periodo}</p>");
            sb.Append($"<p style='font-size: 0.875rem; color: #64748b; margin-top: 0.5rem;'>Generado: {fechaGeneracion:dd/MM/yyyy HH:mm}</p>");
            sb.Append("</div>");

            sb.Append("<div class='reporte-body'>");

            // Ingresos
            sb.AppendFormat("<div class='linea-item'><span>Ingresos por ventas</span><span>{0:C2}</span></div>", ingresosVentas);
            sb.AppendFormat("<div class='linea-item'><span>Menos: Costo de los bienes vendidos</span><span class='{2}'>{0:C2}</span></div>", costoBienesVendidos, 0, costoBienesVendidos < 0 ? "negative" : "");
            sb.AppendFormat("<div class='linea-total'><span>Utilidad bruta</span><span class='{1}'>{0:C2}</span></div>", utilidadBruta, utilidadBruta < 0 ? "negative" : "");

            // Gastos operativos
            sb.AppendFormat("<div class='linea-item'><span>Menos: Gastos operativos</span><span class='{2}'>{0:C2}</span></div>", gastosOperativos, 0, gastosOperativos < 0 ? "negative" : "");
            sb.AppendFormat("<div class='linea-total'><span>Utilidad operativa</span><span class='{1}'>{0:C2}</span></div>", utilidadOperativa, utilidadOperativa < 0 ? "negative" : "");

            // Gastos por intereses
            sb.AppendFormat("<div class='linea-item'><span>Menos: Gastos por intereses</span><span class='{2}'>{0:C2}</span></div>", gastosIntereses, 0, gastosIntereses < 0 ? "negative" : "");
            sb.AppendFormat("<div class='linea-total'><span>Utilidad neta antes de impuestos</span><span class='{1}'>{0:C2}</span></div>", utilidadNetaAntesImpuestos, utilidadNetaAntesImpuestos < 0 ? "negative" : "");

            // Impuestos
            sb.AppendFormat("<div class='linea-item'><span>Menos: Impuestos (tasa = {2}%)</span><span class='{3}'>{0:C2}</span></div>", impuestos, tasaImpuestos, tasaImpuestos.ToString("N2"), impuestos < 0 ? "negative" : "");
            sb.AppendFormat("<div class='linea-total'><span>Utilidad neta despu&eacute;s de impuestos</span><span class='{1}'>{0:C2}</span></div>", utilidadNetaDespuesImpuestos, utilidadNetaDespuesImpuestos < 0 ? "negative" : "");

            // Dividendos
            sb.AppendFormat("<div class='linea-item'><span>Menos: Dividendos en efectivo</span><span class='{2}'>{0:C2}</span></div>", dividendosEfectivo, 0, dividendosEfectivo < 0 ? "negative" : "");
            
            // Ganancias retenidas (línea final)
            string claseFinal = gananciasRetenidas < 0 ? "linea-final negative" : "linea-final";
            sb.AppendFormat("<div class='{2}'><span>A ganancias retenidas</span><span>{0:C2}</span></div>", gananciasRetenidas, 0, claseFinal);

            sb.Append("</div>"); // Cierra reporte-body

            // Mensaje de estado
            if (gananciasRetenidas >= 0)
            {
                sb.Append("<div class='utilidad-ok'><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='#059669' stroke-width='3' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M20 6L9 17l-5-5'/></svg>Estado de Resultados calculado correctamente. El negocio muestra utilidades.</div>");
            }
            else
            {
                sb.Append("<div class='utilidad-ok' style='background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%); color: #991b1b; border: 2px solid #fecaca; box-shadow: 0 2px 8px rgba(220, 38, 38, 0.15);'><svg class='icon-finance-inline' width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='vertical-align: middle; margin-right: 0.5em;'><path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z'/><line x1='12' y1='9' x2='12' y2='13'/><line x1='12' y1='17' x2='12.01' y2='17'/></svg>Estado de Resultados muestra pérdida. Se recomienda revisar los costos y gastos.</div>");
            }

            sb.Append("</div>"); // Cierra reporte-container
            ltlReporte.Text = sb.ToString();
        }
    }
}
