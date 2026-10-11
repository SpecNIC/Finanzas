using System;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;       // Para conectar a SQL Server
using System.Configuration;      // Para leer el Web.config
using System.Web.Security;       // Para manejar la autenticación

namespace Software
{
    public partial class Login : System.Web.UI.Page
    {
        // Variable para guardar la cadena de conexión
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Asegurarse de que los campos estén completamente vacíos
                txtUsuario.Text = "";
                txtPassword.Text = "";
                
                // Deshabilitar autocompletado para que no aparezcan datos guardados
                txtUsuario.Attributes.Add("autocomplete", "off");
                txtPassword.Attributes.Add("autocomplete", "off");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Sanitizar entrada del usuario
            string usuario = SecurityHelper.SanitizeInput(txtUsuario.Text);
            string passwordIngresada = txtPassword.Text; // No sanitizar contraseña (puede tener caracteres especiales)

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(passwordIngresada))
            {
                lblError.Text = "Por favor, ingrese usuario y contraseña.";
                lblError.Visible = true;
                return;
            }

            // Verificar si la cuenta está bloqueada por demasiados intentos fallidos
            if (SecurityHelper.IsAccountLocked(usuario, Context))
            {
                int remainingMinutes = SecurityHelper.GetRemainingLockoutMinutes(usuario, Context);
                lblError.Text = $"Cuenta temporalmente bloqueada por múltiples intentos fallidos. Intente nuevamente en {remainingMinutes} minuto(s).";
                lblError.Visible = true;
                return;
            }

            // 2. Conectar a la BD y verificar
            // El 'using' se encarga de abrir y cerrar la conexión automáticamente
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Escribimos la consulta SQL segura (con parámetros)
                // Ahora solo obtenemos el hash almacenado, no comparamos directamente
                string query = "SELECT UsuarioID, NombreCompleto, EsAdmin, PasswordHash FROM Usuarios WHERE NombreUsuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Añadimos los parámetros para evitar Inyección SQL
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    try
                    {
                        con.Open(); // Abrimos la conexión

                        // ExecuteReader se usa porque esperamos que devuelva datos (un SELECT)
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows) // ¿Encontramos un usuario?
                        {
                            reader.Read(); // Leemos la fila encontrada

                            // Obtener el hash almacenado
                            string storedHash = reader["PasswordHash"].ToString();

                            // Verificar contraseña usando SecurityHelper (soporta PBKDF2 y SHA256 legacy)
                            bool passwordCorrect = SecurityHelper.VerifyPassword(passwordIngresada, storedHash);

                            if (passwordCorrect)
                            {
                                // Limpiar intentos fallidos después de login exitoso
                                SecurityHelper.ClearFailedLoginAttempts(usuario, Context);

                                // Guardar datos del usuario en la Sesión para usarlos en otras páginas
                                Session["UsuarioID"] = reader["UsuarioID"].ToString();
                                Session["NombreCompleto"] = reader["NombreCompleto"].ToString();
                                Session["EsAdmin"] = (bool)reader["EsAdmin"];

                                // Si el hash es antiguo (SHA256), actualizarlo a PBKDF2
                                if (!storedHash.Contains(":"))
                                {
                                    ActualizarHashPassword(usuario, passwordIngresada);
                                }

                                // ¡Inicio de sesión exitoso!
                                Response.Redirect("Menu.aspx"); // Redirigimos al menú principal
                            }
                            else
                            {
                                // Contraseña incorrecta - registrar intento fallido
                                SecurityHelper.RecordFailedLoginAttempt(usuario, Context);
                                lblError.Text = "Usuario o contraseña incorrectos.";
                                lblError.Visible = true;
                            }
                        }
                        else
                        {
                            // Usuario no existe - registrar intento fallido (pero no revelar que el usuario no existe)
                            SecurityHelper.RecordFailedLoginAttempt(usuario, Context);
                            lblError.Text = "Usuario o contraseña incorrectos.";
                            lblError.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Capturamos cualquier error de la base de datos
                        lblError.Text = "Error al conectar con el servidor. Por favor, intente más tarde.";
                        lblError.Visible = true;
                        // En producción, no mostrar detalles del error al usuario
                        System.Diagnostics.Debug.WriteLine($"Error de login: {ex.Message}");
                    }
                } // El 'using' de SqlCommand se cierra aquí
            } // El 'using' de SqlConnection se cierra aquí
        }

        /// <summary>
        /// Actualiza el hash de contraseña de SHA256 legacy a PBKDF2
        /// </summary>
        private void ActualizarHashPassword(string usuario, string password)
        {
            try
            {
                string nuevoHash = SecurityHelper.HashPassword(password);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Usuarios SET PasswordHash = @PasswordHash WHERE NombreUsuario = @Usuario";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", nuevoHash);
                        cmd.Parameters.AddWithValue("@Usuario", usuario);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Si falla la actualización, no es crítico - el login ya fue exitoso
                // El hash se actualizará en el próximo login
            }
        }
    }
}