using System;
using System.Web.UI;
using System.Web.UI.WebControls;
// --- ¡Añade estos 'using'! ---
using System.Data; // Para el DataTable
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization; // Para convertir decimales

namespace Software
{
    public partial class CrearAsiento : System.Web.UI.Page
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
                // Si es la primera vez que carga la página:
                // 1. Llena el menú desplegable de cuentas
                PoblarCuentas();

                // 2. Prepara la tabla temporal para las líneas
                InicializarGridTemporal();

                // 3. Pone la fecha de hoy por defecto
                txtFechaAsiento.Text = DateTime.Now.ToString("yyyy-MM-dd"); 
            }
        }

        /// <summary>
        /// Lee el 'CatalogoCuentas' de la BD y lo carga en el DropDownList.
        /// </summary>
        private void PoblarCuentas()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Seleccionamos solo las cuentas que reciben movimientos
                    // (Asumimos que todas pueden, si no, necesitarías un campo 'EsTransaccional' en tu tabla)
                    string query = "SELECT CuentaID, (CodigoCuenta + ' - ' + NombreCuenta) AS NombreMostrado FROM CatalogoCuentas ORDER BY CodigoCuenta";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlCuentas.DataSource = dt;
                    ddlCuentas.DataTextField = "NombreMostrado";
                    ddlCuentas.DataValueField = "CuentaID";
                    ddlCuentas.DataBind();

                    // Al 14/03/26. El valor '0' se estaba asociando a "Seleccione una cuenta" en vez de asociarse a "Efectivo y bancos"
                    ddlCuentas.Items.Insert(0, new ListItem("-- Seleccione una cuenta --", " "));
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error CRÍTICO al cargar cuentas: " + ex.Message + ". No se puede continuar.", true);
                    // Deshabilitamos todo si no hay cuentas
                    btnAgregarLinea.Enabled = false;
                    ddlCuentas.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Crea un DataTable vacío en el ViewState para guardar las líneas temporalmente.
        /// </summary>
        private void InicializarGridTemporal()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("IDTemporal", typeof(Guid)); // ID único para poder borrar
            dt.Columns.Add("CuentaID", typeof(int));
            dt.Columns.Add("NombreCuenta", typeof(string));
            dt.Columns.Add("Debe", typeof(decimal));
            dt.Columns.Add("Haber", typeof(decimal));

            // Guarda la estructura en ViewState
            ViewState["DetalleAsiento"] = dt;

            // Enlaza el grid vacío
            gvDetalleAsiento.DataSource = dt;
            gvDetalleAsiento.DataBind();
        }

        /// <summary>
        /// Se dispara al hacer clic en "Agregar Línea".
        /// </summary>
        protected void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            // --- Validaciones ---
            // Al 14/03/26. El valor '0' se estaba asociando a "Seleccione una cuenta" en vez de asociarse a "Efectivo y bancos"
            if (ddlCuentas.SelectedValue == " ")
            {
                MostrarMensaje("Debe seleccionar una cuenta contable.", true);
                return;
            }

            // Usamos 'CultureInfo.InvariantCulture' para leer decimales con '.' sin importar el idioma del servidor
            decimal debe = 0;
            decimal haber = 0;
            decimal.TryParse(txtDebe.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out debe);
            decimal.TryParse(txtHaber.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out haber);

            if (debe < 0 || haber < 0)
            {
                MostrarMensaje("Los montos no pueden ser negativos.", true);
                return;
            }
            if (debe == 0 && haber == 0)
            {
                MostrarMensaje("Debe ingresar un monto en Debe o en Haber.", true);
                return;
            }
            if (debe > 0 && haber > 0)
            {
                MostrarMensaje("No puede ingresar un monto en Debe y Haber en la misma línea.", true);
                return;
            }

            // --- Agregar la línea al DataTable temporal ---

            // 1. Recupera el DataTable del ViewState
            DataTable dt = (DataTable)ViewState["DetalleAsiento"];

            // 2. Crea la nueva fila
            DataRow dr = dt.NewRow();
            dr["IDTemporal"] = Guid.NewGuid(); // ID único
            dr["CuentaID"] = Convert.ToInt32(ddlCuentas.SelectedValue);
            dr["NombreCuenta"] = ddlCuentas.SelectedItem.Text;
            dr["Debe"] = debe;
            dr["Haber"] = haber;
            dt.Rows.Add(dr);

            // 3. Guarda el DataTable actualizado en ViewState
            ViewState["DetalleAsiento"] = dt;

            // 4. Actualiza el GridView
            gvDetalleAsiento.DataSource = dt;
            gvDetalleAsiento.DataBind();

            // 5. Limpia los campos
            LimpiarCamposLinea();

            // 6. Recalcula los totales
            CalcularTotales();
        }

        /// <summary>
        /// Se dispara al hacer clic en "Eliminar" en una fila del GridView.
        /// </summary>
        protected void gvDetalleAsiento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Obtiene el ID único de la fila a borrar
            Guid idTemporal = (Guid)gvDetalleAsiento.DataKeys[e.RowIndex].Value;

            DataTable dt = (DataTable)ViewState["DetalleAsiento"];

            // Busca la fila por su ID y la elimina
            DataRow[] rows = dt.Select("IDTemporal = '" + idTemporal.ToString() + "'");
            if (rows.Length > 0)
            {
                rows[0].Delete();
            }

            ViewState["DetalleAsiento"] = dt;
            gvDetalleAsiento.DataSource = dt;
            gvDetalleAsiento.DataBind();

            CalcularTotales();
        }

        /// <summary>
        /// Guarda el asiento completo (Encabezado y Detalles) en la BD.
        /// </summary>
        protected void btnGuardarAsiento_Click(object sender, EventArgs e)
        {
            // --- Validaciones Finales ---
            if (string.IsNullOrWhiteSpace(txtFechaAsiento.Text))
            {
                MostrarMensaje("La fecha del asiento es obligatoria.", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MostrarMensaje("La descripción del asiento es obligatoria.", true);
                return;
            }

            DataTable dt = (DataTable)ViewState["DetalleAsiento"];
            if (dt.Rows.Count < 2)
            {
                MostrarMensaje("Un asiento debe tener al menos dos líneas (Debe y Haber).", true);
                return;
            }

            // --- Inicio de la Transacción SQL ---
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction(); // ¡CRÍTICO!

                try
                {
                    // 1. Insertar el Encabezado (AsientosDiario)
                    string queryHeader = @"INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID) 
                                           VALUES (@Fecha, @Desc, @UsuarioID); 
                                           SELECT SCOPE_IDENTITY();"; // Devuelve el ID del asiento creado

                    int nuevoAsientoID;
                    using (SqlCommand cmdHeader = new SqlCommand(queryHeader, con, tran))
                    {
                        cmdHeader.Parameters.AddWithValue("@Fecha", txtFechaAsiento.Text);
                        cmdHeader.Parameters.AddWithValue("@Desc", txtDescripcion.Text.Trim());
                        cmdHeader.Parameters.AddWithValue("@UsuarioID", (int)Convert.ToInt32(Session["UsuarioID"]));

                        nuevoAsientoID = Convert.ToInt32(cmdHeader.ExecuteScalar()); // Obtiene el ID
                    }

                    // 2. Insertar las Líneas (DetalleAsientos)
                    string queryDetail = @"INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber) 
                                           VALUES (@AsientoID, @CuentaID, @Debe, @Haber)";

                    using (SqlCommand cmdDetail = new SqlCommand(queryDetail, con, tran))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            cmdDetail.Parameters.Clear();
                            cmdDetail.Parameters.AddWithValue("@AsientoID", nuevoAsientoID);
                            cmdDetail.Parameters.AddWithValue("@CuentaID", (int)dr["CuentaID"]);
                            cmdDetail.Parameters.AddWithValue("@Debe", (decimal)dr["Debe"]);
                            cmdDetail.Parameters.AddWithValue("@Haber", (decimal)dr["Haber"]);

                            cmdDetail.ExecuteNonQuery(); // Inserta la línea
                        }
                    }

                    // 3. Si todo salió bien, confirma la transacción
                    tran.Commit();

                    // Redirige al historial
                    Response.Redirect("LibroMayor.aspx");
                }
                catch (Exception ex)
                {
                    // 4. Si algo falló, revierte TODOS los cambios
                    tran.Rollback();
                    MostrarMensaje("Error al guardar en BD: " + ex.Message, true);
                }
            }
        }


        // --- FUNCIONES ASISTENTE (Helpers) ---

        private void LimpiarCamposLinea()
        {
            ddlCuentas.SelectedIndex = 0;
            txtDebe.Text = "0.00";
            txtHaber.Text = "0.00";
            ddlCuentas.Focus();
        }

        private void CalcularTotales()
        {
            DataTable dt = (DataTable)ViewState["DetalleAsiento"];

            // Calcula las sumas usando el método Compute de DataTable
            decimal totalDebe = (decimal)dt.Compute("SUM(Debe)", string.Empty);
            decimal totalHaber = (decimal)dt.Compute("SUM(Haber)", string.Empty);

            // Establece solo los valores numéricos (sin el label, ya está en HTML)
            lblTotalDebe.Text = totalDebe.ToString("N2");
            lblTotalHaber.Text = totalHaber.ToString("N2");

            // Verifica si está cuadrado
            if (totalDebe == totalHaber && totalDebe > 0)
            {
                lblDescuadre.Visible = false;
                lblTotalDebe.CssClass = "totals-value totals-ok";
                lblTotalHaber.CssClass = "totals-value totals-ok";
                totalsContainer.Attributes["class"] = "totals-inner totals-balanced";
                btnGuardarAsiento.Enabled = true; // Habilita el botón de guardar
            }
            else
            {
                lblDescuadre.Visible = (dt.Rows.Count > 0); // Muestra error si hay filas
                lblTotalDebe.CssClass = "totals-value";
                lblTotalHaber.CssClass = "totals-value";
                totalsContainer.Attributes["class"] = "totals-inner";
                btnGuardarAsiento.Enabled = false; // Deshabilita el botón de guardar
            }
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esError ? "feedback-message error" : "feedback-message success";
            lblMensaje.Visible = true;
        }
    }
}