using System;
using System.Web.UI;
// --- ¡Añade estos 'using'! ---
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Software
{
    public partial class VerAsiento : System.Web.UI.Page
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
                // Verificamos si la URL trae un ID (ej. ...?ID=1)
                if (Request.QueryString["ID"] != null)
                {
                    int asientoID;
                    // Verificamos que el ID sea un número válido
                    if (int.TryParse(Request.QueryString["ID"], out asientoID))
                    {
                        CargarDatosAsiento(asientoID);
                    }
                    else
                    {
                        MostrarMensaje("El ID del asiento no es válido.", true);
                        pnlContenido.Visible = false; // Oculta el panel
                    }
                }
                else
                {
                    MostrarMensaje("No se ha especificado ningún asiento para ver.", true);
                    pnlContenido.Visible = false; // Oculta el panel
                }
            }
        }

        /// <summary>
        /// Carga el encabezado y el detalle del asiento desde la BD.
        /// </summary>
        private void CargarDatosAsiento(int asientoID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Verificar permisos: solo admin puede ver todos, usuarios regulares solo los suyos
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    // --- 1. Cargar el Encabezado (AsientosDiario) ---
                    string queryHeader = "SELECT FechaAsiento, Descripcion, UsuarioID FROM AsientosDiario WHERE AsientoID = @ID";
                    using (SqlCommand cmdHeader = new SqlCommand(queryHeader, con))
                    {
                        cmdHeader.Parameters.AddWithValue("@ID", asientoID);
                        using (SqlDataReader reader = cmdHeader.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int asientoUsuarioID = (int)reader["UsuarioID"];
                                
                                // Verificar permisos
                                if (!esAdmin && asientoUsuarioID != usuarioID)
                                {
                                    MostrarMensaje("No tienes permiso para ver este asiento.", true);
                                    pnlContenido.Visible = false;
                                    return;
                                }
                                
                                // Llenamos los labels del encabezado
                                lblAsientoID.Text = asientoID.ToString();
                                lblFecha.Text = ((DateTime)reader["FechaAsiento"]).ToString("dd/MM/yyyy");
                                lblDescripcion.Text = reader["Descripcion"].ToString();
                            }
                            else
                            {
                                // Si no se encuentra el ID
                                MostrarMensaje("Error: Asiento con ID " + asientoID + " no encontrado.", true);
                                pnlContenido.Visible = false;
                                return;
                            }
                        } // Cierra el DataReader
                    } // Cierra el SqlCommand

                    // --- 2. Cargar el Detalle (DetalleAsientos) ---
                    string queryDetail = @"SELECT 
                                            CC.CodigoCuenta + ' - ' + CC.NombreCuenta AS NombreCuenta, 
                                            DA.Debe, 
                                            DA.Haber 
                                         FROM 
                                            DetalleAsientos DA
                                         JOIN 
                                            CatalogoCuentas CC ON DA.CuentaID = CC.CuentaID
                                         WHERE 
                                            DA.AsientoID = @ID
                                         ORDER BY 
                                            DA.Debe DESC"; // Ordena (Debe primero, Haber después)

                    SqlDataAdapter da = new SqlDataAdapter(queryDetail, con);
                    da.SelectCommand.Parameters.AddWithValue("@ID", asientoID);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Llenamos el GridView
                    gvDetalle.DataSource = dt;
                    gvDetalle.DataBind();

                    // --- 3. Calcular Totales ---
                    decimal totalDebe = 0;
                    decimal totalHaber = 0;

                    // Sumamos los valores del DataTable
                    foreach (DataRow row in dt.Rows)
                    {
                        totalDebe += (decimal)row["Debe"];
                        totalHaber += (decimal)row["Haber"];
                    }

                    lblTotalDebe.Text = totalDebe.ToString("N2");
                    lblTotalHaber.Text = totalHaber.ToString("N2");

                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar el asiento: " + ex.Message, true);
                    pnlContenido.Visible = false;
                }
            }
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