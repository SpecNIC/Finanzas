using System;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Configuration;

namespace Software
{
    public partial class Registro : System.Web.UI.Page
    {
        // Leer la cadena de conexión desde Web.config
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Asegurarse de que los campos estén completamente vacíos
                txtNombreCompleto.Text = "";
                txtCorreo.Text = "";
                txtUsuario.Text = "";
                txtPassword.Text = "";
                txtConfirmarPassword.Text = "";
                
                // Deshabilitar autocompletado para que no aparezcan datos guardados
                txtNombreCompleto.Attributes.Add("autocomplete", "off");
                txtCorreo.Attributes.Add("autocomplete", "off");
                txtUsuario.Attributes.Add("autocomplete", "off");
                txtPassword.Attributes.Add("autocomplete", "off");
                txtConfirmarPassword.Attributes.Add("autocomplete", "off");
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            // 1. Leer y sanitizar todos los datos de los campos
            string nombreCompleto = SecurityHelper.SanitizeInput(txtNombreCompleto.Text);
            string correo = SecurityHelper.SanitizeInput(txtCorreo.Text);
            string usuario = SecurityHelper.SanitizeInput(txtUsuario.Text);
            string passwordIngresada = txtPassword.Text; // No sanitizar contraseña
            string confirmarPassword = txtConfirmarPassword.Text; // No sanitizar contraseña

            // 2. --- VALIDACIONES ---

            // Validación A: Campos vacíos
            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(passwordIngresada))
            {
                MostrarMensaje("Por favor, completa todos los campos obligatorios (Nombre, Usuario, Contraseña).", true);
                return;
            }

            // Validación A.1: Longitud mínima de contraseña
            if (passwordIngresada.Length < 8)
            {
                MostrarMensaje("La contraseña debe tener al menos 8 caracteres.", true);
                return;
            }

            // Validación A.2: Fortaleza de contraseña (opcional pero recomendado)
            if (!SecurityHelper.IsStrongPassword(passwordIngresada))
            {
                MostrarMensaje("La contraseña debe contener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y/o caracteres especiales.", true);
                return;
            }

            // Validación B: Contraseñas no coinciden
            if (passwordIngresada != confirmarPassword)
            {
                MostrarMensaje("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.", true);
                return;
            }

            // Validación C: Validar correo electrónico si se proporciona
            if (!string.IsNullOrWhiteSpace(correo) && !SecurityHelper.IsValidEmail(correo))
            {
                MostrarMensaje("El formato del correo electrónico no es válido.", true);
                return;
            }

            // 3. --- LÓGICA DE BASE DE DATOS ---

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Validación C: Verificar si el usuario ya existe
                    string queryUsuario = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @Usuario";
                    using (SqlCommand cmdCheck = new SqlCommand(queryUsuario, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@Usuario", usuario);
                        int conteo = (int)cmdCheck.ExecuteScalar(); // ExecuteScalar es eficiente para contar

                        if (conteo > 0)
                        {
                            MostrarMensaje("El nombre de usuario '" + usuario + "' ya existe. Por favor, elige otro.", true);
                            return; // Detiene la ejecución
                        }
                    }

                    // 4. --- INSERTAR EL NUEVO USUARIO ---

                    // Hashear la contraseña usando PBKDF2 (más seguro)
                    string passwordHash = SecurityHelper.HashPassword(passwordIngresada);

                    // Preparar el comando INSERT
                    string queryInsert = @"INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, CorreoElectronico, EsAdmin) 
                                           VALUES (@Usuario, @PasswordHash, @NombreCompleto, @Correo, 0)"; // 0 = No es Admin

                    using (SqlCommand cmdInsert = new SqlCommand(queryInsert, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@Usuario", usuario);
                        cmdInsert.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmdInsert.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);

                        // Manejar el correo opcional (enviamos DBNull.Value si está vacío)
                        if (string.IsNullOrWhiteSpace(correo))
                        {
                            cmdInsert.Parameters.AddWithValue("@Correo", DBNull.Value);
                        }
                        else
                        {
                            cmdInsert.Parameters.AddWithValue("@Correo", correo);
                        }

                        cmdInsert.ExecuteNonQuery(); // Ejecuta el INSERT
                    }

                    // ¡Éxito!
                    MostrarMensaje("¡Usuario registrado exitosamente! Ya puedes iniciar sesión.", false);

                    // (Opcional) Limpiar los campos después del registro
                    txtNombreCompleto.Text = "";
                    txtCorreo.Text = "";
                    txtUsuario.Text = "";
                    // Deja los campos de contraseña vacíos por seguridad
                }
                catch (Exception ex)
                {
                    // Capturar cualquier error de SQL
                    MostrarMensaje("Error al conectar con la base de datos: " + ex.Message, true);
                }
            }
        }

        // --- Función Asistente para mostrar mensajes ---
        private void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.Text = mensaje;
            if (esError)
            {
                lblMensaje.CssClass = "message-label error-message";
            }
            else
            {
                lblMensaje.CssClass = "message-label success-message";
            }
            lblMensaje.Visible = true;
        }

    }
}