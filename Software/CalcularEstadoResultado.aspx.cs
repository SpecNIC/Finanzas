using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Software
{
    public partial class CalcularEstadoResultado : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                int añoActual = DateTime.Now.Year;
                txtPeriodo.Text = añoActual.ToString();
            }

           using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // --- Filtro por usuario (mismo criterio que en Menú y Libro Mayor) ---
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;


                    // --- CONSULTA 1: Obteniendo el dato de ventas de SQLServer
                    string totalVentas = @"SELECT SUM(Haber)
                                         FROM DetalleAsientos
                                         WHERE CuentaID = 14;";

                    SqlCommand cmd = new SqlCommand(totalVentas, con);

                    decimal IngresosVentas = Convert.ToDecimal(cmd.ExecuteScalar());
                    txtIngresosVentas.Text = Convert.ToString(IngresosVentas)  ;

                    // --- CONSULTA 2: Obteniendo los costos de ventas de SQLServer

                    string CostosDeVenta = @"SELECT SUM(Debe)
                                         FROM DetalleAsientos
                                         WHERE CuentaID = 17;";

                    SqlCommand cmd2 = new SqlCommand(CostosDeVenta, con);
                    decimal Costos = Convert.ToDecimal(cmd2.ExecuteScalar());
                    txtCostoBienesVendidos.Text = Convert.ToString(Costos);

                    // --- CONSULTA 3: Obteniendo los gastos por interes de SQLServer

                    string GastosInteres = @"SELECT SUM(Debe)
                                         FROM DetalleAsientos
                                         WHERE CuentaID = 19;";

                    SqlCommand cmd3 = new SqlCommand(GastosInteres, con);
                    decimal GastoInteres = Convert.ToDecimal(cmd3.ExecuteScalar());
                    txtGastosIntereses.Text = Convert.ToString(GastoInteres);

                    // --- CONSULTA 4: Obteniendo los gastos operativos (publicidad, servicio básicos, alquiler y salarios)  de SQLServer

                    string GastosOperativos = @"SELECT SUM(Debe)
                                                FROM DetalleAsientos
                                                WHERE CuentaID IN (9, 10, 11, 18);";

                    SqlCommand cmd4 = new SqlCommand(GastosOperativos, con);
                    decimal GastoOperativos = Convert.ToDecimal(cmd4.ExecuteScalar());
                    txtGastosOperativos.Text = Convert.ToString(GastoOperativos);

                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al calcular: " + ex.Message, true);
                    btnGuardarReporte.Visible = false;
                }
            }
        }

        protected void btnCalcular_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPeriodo.Text))
            {
                MostrarMensaje("Por favor, ingresa un per&iacute;odo v&aacute;lido.", true);
                return;
            }

            if (!decimal.TryParse(txtIngresosVentas.Text, out decimal ingresosVentas) || ingresosVentas < 0)
            {
                MostrarMensaje("Por favor, ingresa un valor válido para Ingresos por ventas.", true);
                return;
            }

            if (!decimal.TryParse(txtCostoBienesVendidos.Text, out decimal costoBienesVendidos) || costoBienesVendidos < 0)
            {
                MostrarMensaje("Por favor, ingresa un valor válido para Costo de los bienes vendidos.", true);
                return;
            }

            if (!decimal.TryParse(txtGastosOperativos.Text, out decimal gastosOperativos) || gastosOperativos < 0)
            {
                MostrarMensaje("Por favor, ingresa un valor válido para Gastos operativos.", true);
                return;
            }

            if (!decimal.TryParse(txtGastosIntereses.Text, out decimal gastosIntereses) || gastosIntereses < 0)
            {
                MostrarMensaje("Por favor, ingresa un valor válido para Gastos por intereses.", true);
                return;
            }

            if (!decimal.TryParse(txtTasaImpuestos.Text, out decimal tasaImpuestos) || tasaImpuestos < 0 || tasaImpuestos > 100)
            {
                MostrarMensaje("Por favor, ingresa una tasa de impuestos válida (0-100).", true);
                return;
            }

            if (!decimal.TryParse(txtDividendos.Text, out decimal dividendos) || dividendos < 0)
            {
                MostrarMensaje("Por favor, ingresa un valor válido para Dividendos en efectivo.", true);
                return;
            }

            // CORREGIDO: Cálculos mejorados con validaciones
            decimal utilidadBruta = ingresosVentas - costoBienesVendidos;
            decimal utilidadOperativa = utilidadBruta - gastosOperativos;
            decimal utilidadNetaAntesImpuestos = utilidadOperativa - gastosIntereses;
            
            // CORREGIDO: Los impuestos solo se aplican si hay utilidad (no se aplican a pérdidas)
            decimal impuestos = 0;
            if (utilidadNetaAntesImpuestos > 0 && tasaImpuestos > 0)
            {
                impuestos = utilidadNetaAntesImpuestos * (tasaImpuestos / 100);
            }
            
            decimal utilidadNetaDespuesImpuestos = utilidadNetaAntesImpuestos - impuestos;
            
            // CORREGIDO: Validación para dividendos (no pueden exceder la utilidad)
            decimal gananciasRetenidas = utilidadNetaDespuesImpuestos - dividendos;
            if (gananciasRetenidas < 0)
            {
                MostrarMensaje("Advertencia: Los dividendos exceden la utilidad neta después de impuestos. Las ganancias retenidas serán negativas.", true);
            }

            GenerarReporteHTML(ingresosVentas, costoBienesVendidos, gastosOperativos, gastosIntereses, 
                tasaImpuestos, impuestos, dividendos, utilidadBruta, utilidadOperativa, 
                utilidadNetaAntesImpuestos, utilidadNetaDespuesImpuestos, gananciasRetenidas);

            Session["NuevoEstadoResultado_IngresosVentas"] = ingresosVentas;
            Session["NuevoEstadoResultado_CostoBienesVendidos"] = costoBienesVendidos;
            Session["NuevoEstadoResultado_GastosOperativos"] = gastosOperativos;
            Session["NuevoEstadoResultado_GastosIntereses"] = gastosIntereses;
            Session["NuevoEstadoResultado_TasaImpuestos"] = tasaImpuestos;
            Session["NuevoEstadoResultado_DividendosEfectivo"] = dividendos;
            Session["NuevoEstadoResultado_UtilidadBruta"] = utilidadBruta;
            Session["NuevoEstadoResultado_UtilidadOperativa"] = utilidadOperativa;
            Session["NuevoEstadoResultado_UtilidadNetaAntesImpuestos"] = utilidadNetaAntesImpuestos;
            Session["NuevoEstadoResultado_Impuestos"] = impuestos;
            Session["NuevoEstadoResultado_UtilidadNetaDespuesImpuestos"] = utilidadNetaDespuesImpuestos;
            Session["NuevoEstadoResultado_GananciasRetenidas"] = gananciasRetenidas;
            Session["NuevoEstadoResultadoPeriodo"] = txtPeriodo.Text.Trim();

            btnGuardarReporte.Visible = true;
            MostrarMensaje("Cálculo completado. Revisa el reporte y guárdalo.", false);
        }

        protected void btnGuardarReporte_Click(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null || Session["NuevoEstadoResultadoPeriodo"] == null)
            {
                MostrarMensaje("Error: La sesión ha expirado. Por favor, calcula el estado de nuevo.", true);
                return;
            }

            string periodo = Session["NuevoEstadoResultadoPeriodo"].ToString();
            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            decimal ingresosVentas = (decimal)Session["NuevoEstadoResultado_IngresosVentas"];
            decimal costoBienesVendidos = (decimal)Session["NuevoEstadoResultado_CostoBienesVendidos"];
            decimal gastosOperativos = (decimal)Session["NuevoEstadoResultado_GastosOperativos"];
            decimal gastosIntereses = (decimal)Session["NuevoEstadoResultado_GastosIntereses"];
            decimal tasaImpuestos = (decimal)Session["NuevoEstadoResultado_TasaImpuestos"];
            decimal dividendosEfectivo = (decimal)Session["NuevoEstadoResultado_DividendosEfectivo"];
            decimal utilidadBruta = (decimal)Session["NuevoEstadoResultado_UtilidadBruta"];
            decimal utilidadOperativa = (decimal)Session["NuevoEstadoResultado_UtilidadOperativa"];
            decimal utilidadNetaAntesImpuestos = (decimal)Session["NuevoEstadoResultado_UtilidadNetaAntesImpuestos"];
            decimal impuestos = (decimal)Session["NuevoEstadoResultado_Impuestos"];
            decimal utilidadNetaDespuesImpuestos = (decimal)Session["NuevoEstadoResultado_UtilidadNetaDespuesImpuestos"];
            decimal gananciasRetenidas = (decimal)Session["NuevoEstadoResultado_GananciasRetenidas"];

            bool exitoGuardado = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    string queryHeader = "INSERT INTO EstadosResultados (Periodo, UsuarioID) VALUES (@Periodo, @UsuarioID); SELECT SCOPE_IDENTITY();";
                    int nuevoEstadoID;

                    using (SqlCommand cmdHeader = new SqlCommand(queryHeader, con, tran))
                    {
                        cmdHeader.Parameters.AddWithValue("@Periodo", periodo);
                        cmdHeader.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        nuevoEstadoID = Convert.ToInt32(cmdHeader.ExecuteScalar());
                    }

                    string queryDetail = @"INSERT INTO DetalleEstadoResultado 
                        (EstadoResultadoID, IngresosVentas, CostoBienesVendidos, GastosOperativos, GastosIntereses, 
                         TasaImpuestos, DividendosEfectivo, UtilidadBruta, UtilidadOperativa, UtilidadNetaAntesImpuestos, 
                         Impuestos, UtilidadNetaDespuesImpuestos, GananciasRetenidas) 
                        VALUES 
                        (@ID, @IngresosVentas, @CostoBienesVendidos, @GastosOperativos, @GastosIntereses, 
                         @TasaImpuestos, @DividendosEfectivo, @UtilidadBruta, @UtilidadOperativa, @UtilidadNetaAntesImpuestos, 
                         @Impuestos, @UtilidadNetaDespuesImpuestos, @GananciasRetenidas)";

                    using (SqlCommand cmdDetail = new SqlCommand(queryDetail, con, tran))
                    {
                        cmdDetail.Parameters.AddWithValue("@ID", nuevoEstadoID);
                        cmdDetail.Parameters.AddWithValue("@IngresosVentas", ingresosVentas);
                        cmdDetail.Parameters.AddWithValue("@CostoBienesVendidos", costoBienesVendidos);
                        cmdDetail.Parameters.AddWithValue("@GastosOperativos", gastosOperativos);
                        cmdDetail.Parameters.AddWithValue("@GastosIntereses", gastosIntereses);
                        cmdDetail.Parameters.AddWithValue("@TasaImpuestos", tasaImpuestos);
                        cmdDetail.Parameters.AddWithValue("@DividendosEfectivo", dividendosEfectivo);
                        cmdDetail.Parameters.AddWithValue("@UtilidadBruta", utilidadBruta);
                        cmdDetail.Parameters.AddWithValue("@UtilidadOperativa", utilidadOperativa);
                        cmdDetail.Parameters.AddWithValue("@UtilidadNetaAntesImpuestos", utilidadNetaAntesImpuestos);
                        cmdDetail.Parameters.AddWithValue("@Impuestos", impuestos);
                        cmdDetail.Parameters.AddWithValue("@UtilidadNetaDespuesImpuestos", utilidadNetaDespuesImpuestos);
                        cmdDetail.Parameters.AddWithValue("@GananciasRetenidas", gananciasRetenidas);

                        cmdDetail.ExecuteNonQuery();
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
                LimpiarSesion();
                Response.Redirect("EstadoResultado.aspx");
            }
        }

        private void GenerarReporteHTML(decimal ingresosVentas, decimal costoBienesVendidos, decimal gastosOperativos, 
            decimal gastosIntereses, decimal tasaImpuestos, decimal impuestos, decimal dividendos, 
            decimal utilidadBruta, decimal utilidadOperativa, decimal utilidadNetaAntesImpuestos, 
            decimal utilidadNetaDespuesImpuestos, decimal gananciasRetenidas)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='reporte-container'>");
            sb.Append("<h3>Estado de Resultados</h3>");

            sb.AppendFormat("<div class='linea-item'><span>Ingresos por ventas</span><span>${0:N2}</span></div>", ingresosVentas);
            sb.AppendFormat("<div class='linea-item'><span>Menos: Costo de los bienes vendidos</span><span>${0:N2}</span></div>", costoBienesVendidos);
            sb.AppendFormat("<div class='linea-total'><span>Utilidad bruta</span><span>${0:N2}</span></div>", utilidadBruta);

            sb.AppendFormat("<div class='linea-item'><span>Menos: Gastos operativos</span><span>${0:N2}</span></div>", gastosOperativos);
            sb.AppendFormat("<div class='linea-total'><span>Utilidad operativa</span><span>${0:N2}</span></div>", utilidadOperativa);

            sb.AppendFormat("<div class='linea-item'><span>Menos: Gastos por intereses</span><span>${0:N2}</span></div>", gastosIntereses);
            sb.AppendFormat("<div class='linea-total'><span>Utilidad neta antes de impuestos</span><span>${0:N2}</span></div>", utilidadNetaAntesImpuestos);

            sb.AppendFormat("<div class='linea-item'><span>Menos: Impuestos (tasa = {0}%)</span><span>${1:N2}</span></div>", tasaImpuestos, impuestos);
            sb.AppendFormat("<div class='linea-total'><span>Utilidad neta después de impuestos</span><span>${0:N2}</span></div>", utilidadNetaDespuesImpuestos);

            sb.AppendFormat("<div class='linea-item'><span>Menos: Dividendos en efectivo</span><span>${0:N2}</span></div>", dividendos);
            sb.AppendFormat("<div class='linea-total' style='border-top: 3px double #2c5aa0;'><span>A ganancias retenidas</span><span>${0:N2}</span></div>", gananciasRetenidas);

            if (gananciasRetenidas >= 0)
            {
                sb.Append("<div class='utilidad-ok'>Estado de Resultados calculado correctamente.</div>");
            }
            else
            {
                sb.Append("<div class='utilidad-ok' style='background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%); color: #721c24; border-left-color: #dc3545;'>Estado de Resultados muestra pérdida.</div>");
            }

            sb.Append("</div>");
            ltlEstadoReporte.Text = sb.ToString();
        }

        private void LimpiarSesion()
        {
            Session.Remove("NuevoEstadoResultadoPeriodo");
            Session.Remove("NuevoEstadoResultado_IngresosVentas");
            Session.Remove("NuevoEstadoResultado_CostoBienesVendidos");
            Session.Remove("NuevoEstadoResultado_GastosOperativos");
            Session.Remove("NuevoEstadoResultado_GastosIntereses");
            Session.Remove("NuevoEstadoResultado_TasaImpuestos");
            Session.Remove("NuevoEstadoResultado_DividendosEfectivo");
            Session.Remove("NuevoEstadoResultado_UtilidadBruta");
            Session.Remove("NuevoEstadoResultado_UtilidadOperativa");
            Session.Remove("NuevoEstadoResultado_UtilidadNetaAntesImpuestos");
            Session.Remove("NuevoEstadoResultado_Impuestos");
            Session.Remove("NuevoEstadoResultado_UtilidadNetaDespuesImpuestos");
            Session.Remove("NuevoEstadoResultado_GananciasRetenidas");
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}
