using System;
using System.Web.UI;
// --- ¡Añade estos 'using'! ---
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Software
{
    public partial class LibroMayor : System.Web.UI.Page
    {
        // Leer la cadena de conexión desde Web.config
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
            // --- VERIFICACIÓN DE SEGURIDAD ---
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                // Cargar ambos historiales
                CargarAsientos();
                CargarActivosFijos();
            }
        }

        /// <summary>
        /// Carga el historial de asientos (Libro Diario) en el primer GridView.
        /// </summary>
        private void CargarAsientos()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    // Hacemos JOIN con Usuarios para mostrar el nombre del autor
                    string query = @"SELECT 
                                        A.AsientoID, 
                                        A.FechaAsiento, 
                                        A.Descripcion, 
                                        U.NombreCompleto AS Autor, 
                                        A.FechaRegistro 
                                     FROM 
                                        AsientosDiario A
                                     JOIN 
                                        Usuarios U ON A.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR A.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        A.FechaAsiento DESC, A.AsientoID DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvAsientos.DataSource = dt;
                    gvAsientos.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar asientos: " + ex.Message, true);
                }
            }
        }

        protected void gvAsientos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool esAdmin = EsAdministrador();
                LinkButton btnEliminar = e.Row.FindControl("btnEliminarAsiento") as LinkButton;
                
                // Solo administradores pueden eliminar
                if (btnEliminar != null)
                {
                    btnEliminar.Visible = esAdmin;
                }
            }
        }

        protected void gvActivosFijos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool esAdmin = EsAdministrador();
                LinkButton btnEliminar = e.Row.FindControl("btnEliminarActivo") as LinkButton;
                
                // Solo administradores pueden eliminar
                if (btnEliminar != null)
                {
                    btnEliminar.Visible = esAdmin;
                }
            }
        }

        protected void btnEliminarAsiento_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int asientoID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Eliminar primero los detalles (cascada) y luego el asiento
                    string queryDelete = "DELETE FROM AsientosDiario WHERE AsientoID = @AsientoID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@AsientoID", asientoID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Asiento eliminado correctamente.", false);
                            CargarAsientos();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el asiento.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el asiento: " + ex.Message, true);
                }
            }
        }

        protected void btnEditarAsiento_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int asientoID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Eliminar primero los detalles (cascada) y luego el asiento
                    string queryDelete = "DELETE FROM AsientosDiario WHERE AsientoID = @AsientoID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@AsientoID", asientoID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Asiento eliminado correctamente.", false);
                            CargarAsientos();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el asiento.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el asiento: " + ex.Message, true);
                }
            }
        }

        protected void btnEliminarActivo_Command(object sender, CommandEventArgs e)
        {
            // Validar permisos de administrador
            if (!EsAdministrador())
            {
                MostrarMensaje("No tiene permisos para realizar esta acción.", true);
                return;
            }

            int activoID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string queryDelete = "DELETE FROM ActivosFijos WHERE ActivoID = @ActivoID";
                    using (SqlCommand cmd = new SqlCommand(queryDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@ActivoID", activoID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Activo fijo eliminado correctamente.", false);
                            CargarActivosFijos();
                        }
                        else
                        {
                            MostrarMensaje("No se pudo eliminar el activo fijo.", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al eliminar el activo fijo: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Carga el historial de activos fijos en el segundo GridView.
        /// </summary>
        private void CargarActivosFijos()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"SELECT 
                                        ActivoID, 
                                        NombreActivo, 
                                        FechaAdquisicion, 
                                        CostoAdquisicion, 
                                        VidaUtilAnios 
                                     FROM 
                                        ActivosFijos
                                     ORDER BY 
                                        FechaAdquisicion DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvActivosFijos.DataSource = dt;
                    gvActivosFijos.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar activos fijos: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Redirige a la página para crear un nuevo asiento contable.
        /// </summary>
        protected void btnNuevoAsiento_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearAsiento.aspx");
        }

        /// <summary>
        /// Redirige a la página para registrar un nuevo activo fijo.
        /// </summary>
        protected void btnNuevoActivo_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegistrarActivo.aspx");
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