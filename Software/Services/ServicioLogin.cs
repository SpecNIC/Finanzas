using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Software.Services
{
    public class ResultadoLogin
    {
        public bool Exito { get; set; }
        public string MensajeError { get; set; }
        public int? UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public bool EsAdmin { get; set; }
    }

    //Servicio de log en el sistema
    public class ServicioLogin
    {
        private readonly string connectionString;

        public ServicioLogin(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ResultadoLogin ValidarLogin(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                return new ResultadoLogin
                {
                    Exito = false,
                    MensajeError = "Por favor, ingrese usuario y contraseña."
                };
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT UsuarioID, NombreUsuario, EsAdmin, PasswordHash 
                                 FROM Usuarios 
                                 WHERE NombreUsuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Comprobar que el usuario existe
                        if (!reader.HasRows)
                        {
                            return new ResultadoLogin
                            {
                                Exito = false,
                                MensajeError = "Usuario o contraseña incorrectos."
                            };
                        }

                        reader.Read();

                        string storedHash = reader["PasswordHash"].ToString();

                        bool passwordCorrect = SecurityHelper.VerifyPassword(password, storedHash);

                        //Comprobar que la contraseña es correcta
                        if (!passwordCorrect)
                        {
                            return new ResultadoLogin
                            {
                                Exito = false,
                                MensajeError = "Usuario o contraseña incorrectos."
                            };
                        }

                        return new ResultadoLogin
                        {
                            Exito = true,
                            UsuarioID = Convert.ToInt32(reader["UsuarioID"]),
                            NombreUsuario = reader["NombreUsuario"].ToString(),
                            EsAdmin = Convert.ToBoolean(reader["EsAdmin"])
                        };
                    }
                }
            }
        }
    }
}
