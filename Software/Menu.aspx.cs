using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace Software
{
    public partial class Menu : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string activeTab = "balance";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    string nombreCompleto = Session["NombreCompleto"] != null ? Session["NombreCompleto"].ToString() : "Usuario";
                    lblBienvenida.Text = "Hola " + nombreCompleto.Split(' ')[0];
                    
                    // Cargar el historial del tab activo por defecto
                    activeTab = ViewState["ActiveTab"] != null ? ViewState["ActiveTab"].ToString() : "balance";
                    CargarHistoriales();
                    ActivarTab(activeTab);
                }
            }
        }

        private void CargarHistoriales()
        {
            CargarBalances();
            CargarEstadosResultado();
            CargarEstadosOrigen();
            CargarLibroMayor();
            CargarReporteFinal();
        }

        private void CargarBalances()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string query = @"SELECT TOP 10
                                        B.BalanceID, 
                                        B.FechaReporte, 
                                        U.NombreCompleto AS Autor, 
                                        B.FechaGeneracion 
                                     FROM 
                                        BalancesGenerales B
                                     JOIN 
                                        Usuarios U ON B.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR B.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        B.FechaReporte DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvBalance.DataSource = dt;
                    gvBalance.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso para no interrumpir la carga
                }
            }
        }

        private void CargarEstadosResultado()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string query = @"SELECT TOP 10
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

                    gvEstadoResultado.DataSource = dt;
                    gvEstadoResultado.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarEstadosOrigen()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string query = @"SELECT TOP 10
                                        E.EstadoFlujoID, 
                                        E.FechaInicio,
                                        E.FechaFin,
                                        U.NombreCompleto AS Autor, 
                                        E.FechaGeneracion 
                                     FROM 
                                        EstadosFlujo E
                                     JOIN 
                                        Usuarios U ON E.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        E.FechaFin DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvEstadoOrigen.DataSource = dt;
                    gvEstadoOrigen.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarLibroMayor()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
                    int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
                    
                    string query = @"SELECT TOP 10
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

                    gvLibroMayor.DataSource = dt;
                    gvLibroMayor.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void ActivarTab(string tab)
        {
            // Remover clase active de todos los tabs y panels
            tabBalance.CssClass = "tab-button";
            tabEstadoResultado.CssClass = "tab-button";
            tabEstadoOrigen.CssClass = "tab-button";
            tabLibroMayor.CssClass = "tab-button";
            tabReporteFinal.CssClass = "tab-button";
            
            panelBalance.CssClass = "tab-panel";
            panelEstadoResultado.CssClass = "tab-panel";
            panelEstadoOrigen.CssClass = "tab-panel";
            panelLibroMayor.CssClass = "tab-panel";
            panelReporteFinal.CssClass = "tab-panel";

            // Activar el tab seleccionado
            switch (tab)
            {
                case "balance":
                    tabBalance.CssClass = "tab-button active";
                    panelBalance.CssClass = "tab-panel active";
                    break;
                case "resultado":
                    tabEstadoResultado.CssClass = "tab-button active";
                    panelEstadoResultado.CssClass = "tab-panel active";
                    break;
                case "origen":
                    tabEstadoOrigen.CssClass = "tab-button active";
                    panelEstadoOrigen.CssClass = "tab-panel active";
                    break;
                case "libro":
                    tabLibroMayor.CssClass = "tab-button active";
                    panelLibroMayor.CssClass = "tab-panel active";
                    break;
                case "reporte":
                    tabReporteFinal.CssClass = "tab-button active";
                    panelReporteFinal.CssClass = "tab-panel active";
                    CargarReporteFinal();
                    break;
            }
        }

        protected void tabBalance_Click(object sender, EventArgs e)
        {
            activeTab = "balance";
            ViewState["ActiveTab"] = activeTab;
            ActivarTab(activeTab);
        }

        protected void tabEstadoResultado_Click(object sender, EventArgs e)
        {
            activeTab = "resultado";
            ViewState["ActiveTab"] = activeTab;
            ActivarTab(activeTab);
        }

        protected void tabEstadoOrigen_Click(object sender, EventArgs e)
        {
            activeTab = "origen";
            ViewState["ActiveTab"] = activeTab;
            ActivarTab(activeTab);
        }

        protected void tabLibroMayor_Click(object sender, EventArgs e)
        {
            activeTab = "libro";
            ViewState["ActiveTab"] = activeTab;
            ActivarTab(activeTab);
        }

        protected void tabReporteFinal_Click(object sender, EventArgs e)
        {
            activeTab = "reporte";
            ViewState["ActiveTab"] = activeTab;
            ActivarTab(activeTab);
        }

        private void CargarReporteFinal()
        {
            bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
            int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;

            // Cargar estadísticas
            CargarEstadisticas(esAdmin, usuarioID);
            
            // Cargar datos de cada módulo
            CargarReporteBalances(esAdmin, usuarioID);
            CargarReporteEstadosResultado(esAdmin, usuarioID);
            CargarReporteEstadosOrigen(esAdmin, usuarioID);
            CargarReporteAsientos(esAdmin, usuarioID);
            CargarReporteActivos(esAdmin, usuarioID);
        }

        private void CargarEstadisticas(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Contar Balances
                    string queryBalances = esAdmin 
                        ? "SELECT COUNT(*) FROM BalancesGenerales"
                        : "SELECT COUNT(*) FROM BalancesGenerales WHERE UsuarioID = @UsuarioID";
                    SqlCommand cmd = new SqlCommand(queryBalances, con);
                    if (!esAdmin) cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    statBalances.InnerText = cmd.ExecuteScalar().ToString();

                    // Contar Estados de Resultados
                    string queryEstados = esAdmin 
                        ? "SELECT COUNT(*) FROM EstadosResultados"
                        : "SELECT COUNT(*) FROM EstadosResultados WHERE UsuarioID = @UsuarioID";
                    cmd.CommandText = queryEstados;
                    cmd.Parameters.Clear();
                    if (!esAdmin) cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    statEstadosResultado.InnerText = cmd.ExecuteScalar().ToString();

                    // Contar Estados de Origen
                    string queryOrigen = esAdmin 
                        ? "SELECT COUNT(*) FROM EstadosFlujo"
                        : "SELECT COUNT(*) FROM EstadosFlujo WHERE UsuarioID = @UsuarioID";
                    cmd.CommandText = queryOrigen;
                    cmd.Parameters.Clear();
                    if (!esAdmin) cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    statEstadosOrigen.InnerText = cmd.ExecuteScalar().ToString();

                    // Contar Asientos
                    string queryAsientos = esAdmin 
                        ? "SELECT COUNT(*) FROM AsientosDiario"
                        : "SELECT COUNT(*) FROM AsientosDiario WHERE UsuarioID = @UsuarioID";
                    cmd.CommandText = queryAsientos;
                    cmd.Parameters.Clear();
                    if (!esAdmin) cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    statAsientos.InnerText = cmd.ExecuteScalar().ToString();

                    // Contar Activos (todos los usuarios pueden ver todos los activos)
                    string queryActivos = "SELECT COUNT(*) FROM ActivosFijos";
                    cmd.CommandText = queryActivos;
                    cmd.Parameters.Clear();
                    statActivos.InnerText = cmd.ExecuteScalar().ToString();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarReporteBalances(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"SELECT 
                                        B.BalanceID, 
                                        B.FechaReporte, 
                                        U.NombreCompleto AS Autor, 
                                        B.FechaGeneracion 
                                     FROM 
                                        BalancesGenerales B
                                     JOIN 
                                        Usuarios U ON B.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR B.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        B.FechaReporte DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvReporteBalances.DataSource = dt;
                    gvReporteBalances.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarReporteEstadosResultado(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
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

                    gvReporteEstadosResultado.DataSource = dt;
                    gvReporteEstadosResultado.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarReporteEstadosOrigen(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"SELECT 
                                        E.EstadoFlujoID, 
                                        E.FechaInicio,
                                        E.FechaFin,
                                        U.NombreCompleto AS Autor, 
                                        E.FechaGeneracion 
                                     FROM 
                                        EstadosFlujo E
                                     JOIN 
                                        Usuarios U ON E.UsuarioID = U.UsuarioID
                                     WHERE 
                                        (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                     ORDER BY 
                                        E.FechaFin DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvReporteEstadosOrigen.DataSource = dt;
                    gvReporteEstadosOrigen.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarReporteAsientos(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
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

                    gvReporteAsientos.DataSource = dt;
                    gvReporteAsientos.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        private void CargarReporteActivos(bool esAdmin, int usuarioID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    // Los activos fijos son compartidos, pero podemos filtrar si es necesario
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

                    gvReporteActivos.DataSource = dt;
                    gvReporteActivos.DataBind();
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }
        }

        protected void btnExportarReporteFinal_Click(object sender, EventArgs e)
        {
            bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
            int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;
            string nombreUsuario = Session["NombreCompleto"] != null ? Session["NombreCompleto"].ToString() : "Usuario";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<html><head><meta charset='utf-8'><title>Reporte Final</title>");
            sb.Append("<style>body{font-family:Arial;font-size:12px;}table{border-collapse:collapse;width:100%;margin:20px 0;}th,td{border:1px solid #ddd;padding:8px;text-align:left;}th{background-color:#2c5aa0;color:white;}</style></head><body>");
            
            sb.AppendFormat("<h1>Reporte Final - {0}</h1>", nombreUsuario);
            sb.AppendFormat("<p><strong>Fecha de Generación:</strong> {0:dd/MM/yyyy HH:mm}</p>", DateTime.Now);
            sb.AppendFormat("<p><strong>Tipo de Usuario:</strong> {0}</p>", esAdmin ? "Administrador" : "Usuario Regular");
            sb.Append("<hr>");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string whereClause = esAdmin ? "" : $"WHERE UsuarioID = {usuarioID}";

                // Balances Generales
                sb.Append("<h2>Balances Generales</h2>");
                string queryBalances = $@"SELECT B.BalanceID, B.FechaReporte, U.NombreCompleto AS Autor, B.FechaGeneracion 
                                         FROM BalancesGenerales B
                                         INNER JOIN Usuarios U ON B.UsuarioID = U.UsuarioID
                                         {(esAdmin ? "" : $"WHERE B.UsuarioID = {usuarioID}")}
                                         ORDER BY B.FechaReporte DESC";
                ExportarTabla(queryBalances, con, sb);

                // Estados de Resultados
                sb.Append("<h2>Estados de Resultados</h2>");
                string queryEstados = $@"SELECT E.EstadoResultadoID, E.Periodo, U.NombreCompleto AS Autor, E.FechaGeneracion 
                                        FROM EstadosResultados E
                                        INNER JOIN Usuarios U ON E.UsuarioID = U.UsuarioID
                                        {(esAdmin ? "" : $"WHERE E.UsuarioID = {usuarioID}")}
                                        ORDER BY E.FechaGeneracion DESC";
                ExportarTabla(queryEstados, con, sb);

                // Estados de Origen
                sb.Append("<h2>Estados de Origen y Aplicación</h2>");
                string queryOrigen = $@"SELECT E.EstadoFlujoID, E.FechaInicio, E.FechaFin, U.NombreCompleto AS Autor, E.FechaGeneracion 
                                       FROM EstadosFlujo E
                                       INNER JOIN Usuarios U ON E.UsuarioID = U.UsuarioID
                                       {(esAdmin ? "" : $"WHERE E.UsuarioID = {usuarioID}")}
                                       ORDER BY E.FechaFin DESC";
                ExportarTabla(queryOrigen, con, sb);

                // Asientos
                sb.Append("<h2>Asientos Contables</h2>");
                string queryAsientos = $@"SELECT A.AsientoID, A.FechaAsiento, A.Descripcion, U.NombreCompleto AS Autor, A.FechaRegistro 
                                         FROM AsientosDiario A
                                         INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID
                                         {(esAdmin ? "" : $"WHERE A.UsuarioID = {usuarioID}")}
                                         ORDER BY A.FechaAsiento DESC";
                ExportarTabla(queryAsientos, con, sb);

                // Activos Fijos
                sb.Append("<h2>Activos Fijos</h2>");
                string queryActivos = "SELECT ActivoID, NombreActivo, FechaAdquisicion, CostoAdquisicion, VidaUtilAnios FROM ActivosFijos ORDER BY FechaAdquisicion DESC";
                ExportarTabla(queryActivos, con, sb);
            }

            sb.Append("</body></html>");

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=ReporteFinal_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        private void ExportarTabla(string query, SqlConnection con, System.Text.StringBuilder sb)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        sb.Append("<table>");
                        // Headers
                        sb.Append("<tr>");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            // Mapear nombres de columnas a nombres amigables en español
                            string displayName = MapearNombreColumna(columnName);
                            sb.AppendFormat("<th>{0}</th>", displayName);
                        }
                        sb.Append("</tr>");
                        // Rows
                        while (reader.Read())
                        {
                            sb.Append("<tr>");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                object value = reader[i];
                                string displayValue = value == DBNull.Value ? "" : value.ToString();
                                if (value is DateTime)
                                {
                                    displayValue = ((DateTime)value).ToString("dd/MM/yyyy HH:mm");
                                }
                                else if (value is decimal)
                                {
                                    displayValue = ((decimal)value).ToString("C2");
                                }
                                sb.AppendFormat("<td>{0}</td>", displayValue);
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append("</table>");
                    }
                    else
                    {
                        sb.Append("<p>No hay registros disponibles.</p>");
                    }
                }
            }
        }

        private string MapearNombreColumna(string columnName)
        {
            // Mapear nombres de columnas de la base de datos a nombres amigables en español
            switch (columnName)
            {
                case "VidaUtilAnios":
                    return "Vida &Uacute;til (A&ntilde;os)";
                case "BalanceID":
                    return "ID Balance";
                case "EstadoResultadoID":
                    return "ID Estado";
                case "EstadoFlujoID":
                    return "ID Estado";
                case "AsientoID":
                    return "ID Asiento";
                case "ActivoID":
                    return "ID Activo";
                case "FechaReporte":
                    return "Fecha del Reporte";
                case "FechaGeneracion":
                    return "Fecha de Generaci&oacute;n";
                case "FechaInicio":
                    return "Fecha de Inicio";
                case "FechaFin":
                    return "Fecha de Fin";
                case "FechaAsiento":
                    return "Fecha del Asiento";
                case "FechaRegistro":
                    return "Fecha de Registro";
                case "FechaAdquisicion":
                    return "Fecha de Adquisici&oacute;n";
                case "CostoAdquisicion":
                    return "Costo de Adquisici&oacute;n";
                case "NombreActivo":
                    return "Nombre del Activo";
                case "Descripcion":
                    return "Descripci&oacute;n";
                case "Periodo":
                    return "Per&iacute;odo";
                default:
                    return columnName;
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string termino = txtBusqueda.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(termino))
            {
                panelResultadosBusqueda.Visible = false;
                lblResultadoBusqueda.Visible = false;
                return;
            }

            DataTable resultados = BuscarGlobal(termino);
            
            if (resultados.Rows.Count > 0)
            {
                rptResultadosBusqueda.DataSource = resultados;
                rptResultadosBusqueda.DataBind();
                panelResultadosBusqueda.Visible = true;
                lblResultadoBusqueda.Text = $"Se encontraron {resultados.Rows.Count} resultado(s) para '{termino}'";
                lblResultadoBusqueda.Visible = true;
            }
            else
            {
                panelResultadosBusqueda.Visible = false;
                lblResultadoBusqueda.Text = $"No se encontraron resultados para '{termino}'";
                lblResultadoBusqueda.Visible = true;
                lblResultadoBusqueda.CssClass = "search-results-label no-results";
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            panelResultadosBusqueda.Visible = false;
            lblResultadoBusqueda.Visible = false;
        }

        private DataTable BuscarGlobal(string termino)
        {
            DataTable resultados = new DataTable();
            resultados.Columns.Add("Tipo", typeof(string));
            resultados.Columns.Add("Descripcion", typeof(string));
            resultados.Columns.Add("Fecha", typeof(DateTime));
            resultados.Columns.Add("Autor", typeof(string));
            resultados.Columns.Add("ID", typeof(int));
            resultados.Columns.Add("URL", typeof(string));

            bool esAdmin = Session["EsAdmin"] != null && (bool)Session["EsAdmin"];
            int usuarioID = Session["UsuarioID"] != null ? Convert.ToInt32(Session["UsuarioID"]) : 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string terminoBusqueda = $"%{termino}%";

                    // Buscar en Balances Generales
                    string queryBalance = @"SELECT 'Balance General' AS Tipo,
                                                  CONCAT('Balance al ', FORMAT(FechaReporte, 'dd/MM/yyyy')) AS Descripcion,
                                                  FechaReporte AS Fecha,
                                                  U.NombreCompleto AS Autor,
                                                  B.BalanceID AS ID,
                                                  'VerBalance.aspx?ID=' + CAST(B.BalanceID AS VARCHAR) AS URL
                                           FROM BalancesGenerales B
                                           INNER JOIN Usuarios U ON B.UsuarioID = U.UsuarioID
                                           WHERE (@EsAdmin = 1 OR B.UsuarioID = @UsuarioID)
                                              AND (FORMAT(FechaReporte, 'dd/MM/yyyy') LIKE @Termino
                                              OR U.NombreCompleto LIKE @Termino
                                              OR U.NombreUsuario LIKE @Termino)";

                    using (SqlCommand cmd = new SqlCommand(queryBalance, con))
                    {
                        cmd.Parameters.AddWithValue("@Termino", terminoBusqueda);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = resultados.NewRow();
                                row["Tipo"] = reader["Tipo"];
                                row["Descripcion"] = reader["Descripcion"];
                                row["Fecha"] = reader["Fecha"];
                                row["Autor"] = reader["Autor"];
                                row["ID"] = reader["ID"];
                                row["URL"] = reader["URL"];
                                resultados.Rows.Add(row);
                            }
                        }
                    }

                    // Buscar en Estados de Resultado
                    string queryEstadoResultado = @"SELECT 'Estado de Resultados' AS Tipo,
                                                          CONCAT('Período: ', Periodo) AS Descripcion,
                                                          FechaGeneracion AS Fecha,
                                                          U.NombreCompleto AS Autor,
                                                          E.EstadoResultadoID AS ID,
                                                          'VerEstadoResultado.aspx?ID=' + CAST(E.EstadoResultadoID AS VARCHAR) AS URL
                                                   FROM EstadosResultados E
                                                   INNER JOIN Usuarios U ON E.UsuarioID = U.UsuarioID
                                                   WHERE (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                                      AND (Periodo LIKE @Termino
                                                      OR U.NombreCompleto LIKE @Termino
                                                      OR U.NombreUsuario LIKE @Termino)";

                    using (SqlCommand cmd = new SqlCommand(queryEstadoResultado, con))
                    {
                        cmd.Parameters.AddWithValue("@Termino", terminoBusqueda);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = resultados.NewRow();
                                row["Tipo"] = reader["Tipo"];
                                row["Descripcion"] = reader["Descripcion"];
                                row["Fecha"] = reader["Fecha"];
                                row["Autor"] = reader["Autor"];
                                row["ID"] = reader["ID"];
                                row["URL"] = reader["URL"];
                                resultados.Rows.Add(row);
                            }
                        }
                    }

                    // Buscar en Estados de Origen
                    string queryEstadoOrigen = @"SELECT 'Estado de Origen' AS Tipo,
                                                        CONCAT('Del ', FORMAT(FechaInicio, 'dd/MM/yyyy'), ' al ', FORMAT(FechaFin, 'dd/MM/yyyy')) AS Descripcion,
                                                        FechaFin AS Fecha,
                                                        U.NombreCompleto AS Autor,
                                                        E.EstadoFlujoID AS ID,
                                                        'VerEstadoOrigen.aspx?ID=' + CAST(E.EstadoFlujoID AS VARCHAR) AS URL
                                                 FROM EstadosFlujo E
                                                 INNER JOIN Usuarios U ON E.UsuarioID = U.UsuarioID
                                                 WHERE (@EsAdmin = 1 OR E.UsuarioID = @UsuarioID)
                                                    AND (FORMAT(FechaInicio, 'dd/MM/yyyy') LIKE @Termino
                                                    OR FORMAT(FechaFin, 'dd/MM/yyyy') LIKE @Termino
                                                    OR U.NombreCompleto LIKE @Termino
                                                    OR U.NombreUsuario LIKE @Termino)";

                    using (SqlCommand cmd = new SqlCommand(queryEstadoOrigen, con))
                    {
                        cmd.Parameters.AddWithValue("@Termino", terminoBusqueda);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = resultados.NewRow();
                                row["Tipo"] = reader["Tipo"];
                                row["Descripcion"] = reader["Descripcion"];
                                row["Fecha"] = reader["Fecha"];
                                row["Autor"] = reader["Autor"];
                                row["ID"] = reader["ID"];
                                row["URL"] = reader["URL"];
                                resultados.Rows.Add(row);
                            }
                        }
                    }
                    string queryAsientos = @"SELECT 'Asiento Contable' AS Tipo,
                                                    Descripcion AS Descripcion,
                                                    FechaAsiento AS Fecha,
                                                    U.NombreCompleto AS Autor,
                                                    A.AsientoID AS ID,
                                                    'VerAsiento.aspx?ID=' + CAST(A.AsientoID AS VARCHAR) AS URL
                                             FROM AsientosDiario A
                                             INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID
                                             WHERE (@EsAdmin = 1 OR A.UsuarioID = @UsuarioID)
                                                AND (Descripcion LIKE @Termino
                                                OR FORMAT(FechaAsiento, 'dd/MM/yyyy') LIKE @Termino
                                                OR U.NombreCompleto LIKE @Termino
                                                OR U.NombreUsuario LIKE @Termino)";

                    using (SqlCommand cmd = new SqlCommand(queryAsientos, con))
                    {
                        cmd.Parameters.AddWithValue("@Termino", terminoBusqueda);
                        cmd.Parameters.AddWithValue("@EsAdmin", esAdmin);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = resultados.NewRow();
                                row["Tipo"] = reader["Tipo"];
                                row["Descripcion"] = reader["Descripcion"];
                                row["Fecha"] = reader["Fecha"];
                                row["Autor"] = reader["Autor"];
                                row["ID"] = reader["ID"];
                                row["URL"] = reader["URL"];
                                resultados.Rows.Add(row);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Error silencioso
                }
            }

            // Ordenar por fecha descendente
            DataView dv = resultados.DefaultView;
            dv.Sort = "Fecha DESC";
            return dv.ToTable();
        }

        protected void rptResultadosBusqueda_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalle")
            {
                string url = e.CommandArgument.ToString();
                Response.Redirect(url);
            }
        }

        protected string GetTipoIcon(string tipo)
        {
            switch (tipo)
            {
                case "Balance General":
                    return @"<svg class='tipo-icon' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><path d='M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6'></path></svg>";
                case "Estado de Resultados":
                    return @"<svg class='tipo-icon' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><polyline points='22 6 13.5 14.5 8.5 9.5 2 18'></polyline><polyline points='16 6 22 6 22 12'></polyline></svg>";
                case "Estado de Origen":
                    return @"<svg class='tipo-icon' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><path d='M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z'></path><polyline points='3.27 6.96 12 12.01 20.73 6.96'></polyline><line x1='12' y1='22.08' x2='12' y2='12'></line></svg>";
                case "Asiento Contable":
                    return @"<svg class='tipo-icon' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><path d='M4 19.5A2.5 2.5 0 0 1 6.5 17H20'></path><path d='M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z'></path></svg>";
                default:
                    return @"<svg class='tipo-icon' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><rect x='3' y='3' width='18' height='18' rx='2' ry='2'></rect></svg>";
            }
        }
    }
}
