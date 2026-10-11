using System;
using System.Web.UI;
using System.Web.UI.WebControls;
// --- ¡Añade estos 'using'! ---
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization; // Para convertir decimales

namespace Software
{
    public partial class RegistrarActivo : System.Web.UI.Page
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
                // Pone la fecha de hoy por defecto
                txtFechaAdquisicion.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Llenamos los 3 DropDownLists
                // Filtramos por tipo de cuenta para facilitar la selección
                PoblarCuentasDDL(ddlCuentaActivo, "Activo");
                PoblarCuentasDDL(ddlCuentaGastoDepr, "Gasto");
                PoblarCuentasDDL(ddlCuentaDeprAcum, "Activo"); // La Depr. Acum. es una contra-cuenta de Activo
            }
        }

        /// <summary>
        /// Llena un DropDownList específico con cuentas de un tipo específico.
        /// </summary>
        private void PoblarCuentasDDL(DropDownList ddl, string tipoCuenta)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"SELECT CuentaID, (CodigoCuenta + ' - ' + NombreCuenta) AS NombreMostrado 
                                     FROM CatalogoCuentas 
                                     WHERE TipoCuenta = @TipoCuenta 
                                     ORDER BY CodigoCuenta";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@TipoCuenta", tipoCuenta);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddl.DataSource = dt;
                    ddl.DataTextField = "NombreMostrado";
                    ddl.DataValueField = "CuentaID";
                    ddl.DataBind();

                    ddl.Items.Insert(0, new ListItem("-- Seleccione una cuenta de " + tipoCuenta.ToLower() + " --", "0"));
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar cuentas de " + tipoCuenta + ": " + ex.Message, true);
                    ddl.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Guarda el nuevo activo en la base de datos.
        /// </summary>
        protected void btnGuardarActivo_Click(object sender, EventArgs e)
        {
            // --- 1. Validaciones de Campos ---
            if (string.IsNullOrWhiteSpace(txtNombreActivo.Text))
            {
                MostrarMensaje("El nombre del activo es obligatorio.", true);
                return;
            }

            DateTime fechaAdq;
            if (!DateTime.TryParse(txtFechaAdquisicion.Text, out fechaAdq))
            {
                MostrarMensaje("La fecha de adquisición no es válida.", true);
                return;
            }

            // Usamos 'CultureInfo.InvariantCulture' para leer decimales con '.'
            decimal costo, rescate;
            if (!decimal.TryParse(txtCostoAdquisicion.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out costo) || costo <= 0)
            {
                MostrarMensaje("El costo de adquisición debe ser un número mayor a 0.", true);
                return;
            }
            if (!decimal.TryParse(txtValorRescate.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out rescate) || rescate < 0)
            {
                MostrarMensaje("El valor de rescate debe ser un número (0 o mayor).", true);
                return;
            }

            int vidaUtil;
            if (!int.TryParse(txtVidaUtil.Text, out vidaUtil) || vidaUtil <= 0)
            {
                MostrarMensaje("La vida útil debe ser un número entero mayor a 0.", true);
                return;
            }

            // Validar DropDownLists
            if (ddlCuentaActivo.SelectedValue == "0" || ddlCuentaGastoDepr.SelectedValue == "0" || ddlCuentaDeprAcum.SelectedValue == "0")
            {
                MostrarMensaje("Debe seleccionar las tres cuentas contables asociadas.", true);
                return;
            }
            if (ddlCuentaActivo.SelectedValue == ddlCuentaDeprAcum.SelectedValue)
            {
                MostrarMensaje("La cuenta de Activo y la de Depreciación Acumulada no pueden ser la misma.", true);
                return;
            }


            // --- 2. Inserción en la Base de Datos ---
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"INSERT INTO ActivosFijos 
                                        (NombreActivo, Descripcion, FechaAdquisicion, CostoAdquisicion, VidaUtilAnios, ValorRescate, 
                                         CuentaActivoID, CuentaGastoDeprID, CuentaDeprAcumID)
                                     VALUES 
                                        (@Nombre, @Desc, @Fecha, @Costo, @VidaUtil, @Rescate, 
                                         @CuentaActivo, @CuentaGasto, @CuentaDeprAcum)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombreActivo.Text.Trim());

                        // Manejar descripción opcional
                        if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
                        {
                            cmd.Parameters.AddWithValue("@Desc", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Desc", txtDescripcion.Text.Trim());
                        }

                        cmd.Parameters.AddWithValue("@Fecha", fechaAdq);
                        cmd.Parameters.AddWithValue("@Costo", costo);
                        cmd.Parameters.AddWithValue("@VidaUtil", vidaUtil);
                        cmd.Parameters.AddWithValue("@Rescate", rescate);
                        cmd.Parameters.AddWithValue("@CuentaActivo", Convert.ToInt32(ddlCuentaActivo.SelectedValue));
                        cmd.Parameters.AddWithValue("@CuentaGasto", Convert.ToInt32(ddlCuentaGastoDepr.SelectedValue));
                        cmd.Parameters.AddWithValue("@CuentaDeprAcum", Convert.ToInt32(ddlCuentaDeprAcum.SelectedValue));

                        cmd.ExecuteNonQuery(); // Ejecuta el INSERT
                    }

                    // Si todo sale bien, redirige de vuelta
                    Response.Redirect("LibroMayor.aspx");
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al guardar el activo: " + ex.Message, true);
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