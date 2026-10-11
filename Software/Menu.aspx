<%-- 
  Página: Menu.aspx
  Propósito: Dashboard principal de la aplicación.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="Software.Menu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Dashboard - Sistema Financiero</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Styles/Menu.css?v=3.9" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="dashboard-layout">
            
            <!-- Sidebar -->
            <aside class="sidebar">
                <div class="sidebar-header">
                    <div class="app-logo">
                        <div class="logo-icon-wrapper">
                            <svg class="logo-svg" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
                                <defs>
                                    <linearGradient id="logoGradient" x1="0%" y1="0%" x2="100%" y2="100%">
                                        <stop offset="0%" style="stop-color:#ffffff;stop-opacity:1" />
                                        <stop offset="100%" style="stop-color:rgba(255,255,255,0.9);stop-opacity:1" />
                                    </linearGradient>
                                </defs>
                                <circle cx="24" cy="24" r="22" fill="url(#logoGradient)" opacity="0.2"/>
                                <path d="M15 30 L18 24 L21 27 L24 15 L27 20 L30 12 L33 18" 
                                      stroke="white" 
                                      stroke-width="3" 
                                      fill="none" 
                                      stroke-linecap="round" 
                                      stroke-linejoin="round"
                                      opacity="0.95"/>
                                <circle cx="24" cy="32" r="6" fill="rgba(255,255,255,0.3)" stroke="white" stroke-width="2"/>
                                <text x="24" y="36" font-family="Arial, sans-serif" font-size="8" font-weight="bold" fill="white" text-anchor="middle">$</text>
                            </svg>
                        </div>
                        <h1 class="app-title">Finanzas</h1>
                    </div>
                </div>
                
                <div class="user-profile">
                    <div class="user-info">
                        <h2 class="user-name">
                            <asp:Label ID="lblBienvenida" runat="server" Text="Hola Usuario"></asp:Label>
                        </h2>
                        <p class="user-subtitle">Revisa tus reportes financieros</p>
                    </div>
                </div>
                
                <nav class="sidebar-nav">
                    <asp:HyperLink ID="linkDashboard" runat="server" CssClass="nav-item active" NavigateUrl="~/Menu.aspx">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="3" y="3" width="7" height="7"></rect>
                            <rect x="14" y="3" width="7" height="7"></rect>
                            <rect x="14" y="14" width="7" height="7"></rect>
                            <rect x="3" y="14" width="7" height="7"></rect>
                        </svg>
                        <span class="nav-text">Dashboard</span>
                    </asp:HyperLink>
                    
                    <asp:HyperLink ID="linkBalance" runat="server" CssClass="nav-item" NavigateUrl="~/BalanceGeneral.aspx">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
                        </svg>
                        <span class="nav-text">Balance General</span>
                    </asp:HyperLink>
                    
                    <asp:HyperLink ID="linkEstadoResultado" runat="server" CssClass="nav-item" NavigateUrl="~/EstadoResultado.aspx">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="22 6 13.5 14.5 8.5 9.5 2 18"></polyline>
                            <polyline points="16 6 22 6 22 12"></polyline>
                        </svg>
                        <span class="nav-text">Estado de Resultados</span>
                    </asp:HyperLink>
                    
                    <asp:HyperLink ID="linkEstadoOrigen" runat="server" CssClass="nav-item" NavigateUrl="~/EstadoOrigenAplicacion.aspx">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                            <polyline points="3.27 6.96 12 12.01 20.73 6.96"></polyline>
                            <line x1="12" y1="22.08" x2="12" y2="12"></line>
                        </svg>
                        <span class="nav-text">Estado de Origen</span>
                    </asp:HyperLink>
                    
                    <asp:HyperLink ID="linkLibroMayor" runat="server" CssClass="nav-item" NavigateUrl="~/LibroMayor.aspx">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                            <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                        </svg>
                        <span class="nav-text">Libro Mayor</span>
                    </asp:HyperLink>
                </nav>
                
                <!-- Toggle Modo Oscuro -->
                <div class="dark-mode-toggle-container">
                    <label class="dark-mode-toggle-label">
                        <svg class="dark-mode-icon sun-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <circle cx="12" cy="12" r="5"></circle>
                            <line x1="12" y1="1" x2="12" y2="3"></line>
                            <line x1="12" y1="21" x2="12" y2="23"></line>
                            <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
                            <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
                            <line x1="1" y1="12" x2="3" y2="12"></line>
                            <line x1="21" y1="12" x2="23" y2="12"></line>
                            <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
                            <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
                        </svg>
                        <span class="dark-mode-text">Modo Oscuro</span>
                        <input type="checkbox" id="darkModeToggle" class="dark-mode-toggle" />
                        <span class="dark-mode-slider"></span>
                        <svg class="dark-mode-icon moon-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                        </svg>
                    </label>
                </div>
                
                <div class="sidebar-footer">
                    <asp:LinkButton ID="btnCerrarSesion" runat="server" 
                        CssClass="logout-button" 
                        OnClick="btnCerrarSesion_Click">
                        <svg class="nav-icon-svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                            <polyline points="16 17 21 12 16 7"></polyline>
                            <line x1="21" y1="12" x2="9" y2="12"></line>
                        </svg>
                        <span>Cerrar Sesi&oacute;n</span>
                    </asp:LinkButton>
                </div>
            </aside>
            
            <!-- Main Content -->
            <main class="main-content">
                
                <!-- Content Area -->
                <div class="content-area">
                    
                    <div class="page-header">
                        <h1 class="page-title">Overview</h1>
                        <p class="page-subtitle">Historial de reportes financieros</p>
                        
                        <!-- Barra de Búsqueda Global -->
                        <div class="search-container">
                            <asp:TextBox ID="txtBusqueda" runat="server" CssClass="search-input" 
                                placeholder="Buscar en todos los módulos..." 
                                AutoPostBack="false"></asp:TextBox>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" 
                                CssClass="btn-search" OnClick="btnBuscar_Click" />
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" 
                                CssClass="btn-clear" OnClick="btnLimpiar_Click" />
                        </div>
                        
                        <asp:Label ID="lblResultadoBusqueda" runat="server" CssClass="search-results-label" Visible="false"></asp:Label>
                        
                        <!-- Panel de Resultados de Búsqueda -->
                        <asp:Panel ID="panelResultadosBusqueda" runat="server" CssClass="search-results-panel" Visible="false">
                            <div class="search-results-header">
                                <h3 class="search-results-title">
                                    <svg class="search-icon-title" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                        <circle cx="11" cy="11" r="8"></circle>
                                        <path d="m21 21-4.35-4.35"></path>
                                    </svg>
                                    Resultados de Búsqueda
                                </h3>
                            </div>
                            <div class="search-results-cards">
                                <asp:Repeater ID="rptResultadosBusqueda" runat="server" OnItemCommand="rptResultadosBusqueda_ItemCommand">
                                    <ItemTemplate>
                                        <div class="result-card" data-tipo='<%# Eval("Tipo") %>'>
                                            <div class="result-card-header">
                                                <div class="result-type-badge">
                                                    <%# GetTipoIcon((string)Eval("Tipo")) %>
                                                    <span class="result-type-text"><%# Eval("Tipo") %></span>
                                                </div>
                                            </div>
                                            <div class="result-card-body">
                                                <h4 class="result-description"><%# Eval("Descripcion") %></h4>
                                                <div class="result-meta">
                                                    <div class="result-meta-item">
                                                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                            <circle cx="12" cy="12" r="10"></circle>
                                                            <polyline points="12 6 12 12 16 14"></polyline>
                                                        </svg>
                                                        <span><%# ((DateTime)Eval("Fecha")).ToString("dd/MM/yyyy") %></span>
                                                    </div>
                                                    <div class="result-meta-item">
                                                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                                                            <circle cx="12" cy="7" r="4"></circle>
                                                        </svg>
                                                        <span><%# Eval("Autor") %></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="result-card-footer">
                                                <asp:LinkButton ID="lnkVerDetalle" runat="server" 
                                                    CommandName="VerDetalle" 
                                                    CommandArgument='<%# Eval("URL") %>'
                                                    CssClass="result-view-btn">
                                                    Ver Detalle
                                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <polyline points="9 18 15 12 9 6"></polyline>
                                                    </svg>
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>
                    </div>
                    
                    <!-- Tabs Navigation -->
                    <div class="tabs-container">
                        <div class="tabs-nav">
                            <asp:LinkButton ID="tabBalance" runat="server" CssClass="tab-button active" OnClick="tabBalance_Click">
                                <svg class="tab-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
                                </svg>
                                <span>Balance General</span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="tabEstadoResultado" runat="server" CssClass="tab-button" OnClick="tabEstadoResultado_Click">
                                <svg class="tab-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <polyline points="22 6 13.5 14.5 8.5 9.5 2 18"></polyline>
                                    <polyline points="16 6 22 6 22 12"></polyline>
                                </svg>
                                <span>Estado de Resultados</span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="tabEstadoOrigen" runat="server" CssClass="tab-button" OnClick="tabEstadoOrigen_Click">
                                <svg class="tab-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                                    <polyline points="3.27 6.96 12 12.01 20.73 6.96"></polyline>
                                    <line x1="12" y1="22.08" x2="12" y2="12"></line>
                                </svg>
                                <span>Estado de Origen</span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="tabLibroMayor" runat="server" CssClass="tab-button" OnClick="tabLibroMayor_Click">
                                <svg class="tab-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                    <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                                </svg>
                                <span>Libro Mayor</span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="tabReporteFinal" runat="server" CssClass="tab-button" OnClick="tabReporteFinal_Click">
                                <svg class="tab-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
                                    <polyline points="14 2 14 8 20 8"></polyline>
                                    <line x1="16" y1="13" x2="8" y2="13"></line>
                                    <line x1="16" y1="17" x2="8" y2="17"></line>
                                    <polyline points="10 9 9 9 8 9"></polyline>
                                </svg>
                                <span>Reporte Final</span>
                            </asp:LinkButton>
                        </div>
                        
                        <!-- Tab Content -->
                        <div class="tabs-content">
                            
                            <!-- Balance General Tab -->
                            <asp:Panel ID="panelBalance" runat="server" CssClass="tab-panel active">
                                <div class="tab-header">
                                    <h2>Historial de Balances Generales</h2>
                                    <asp:HyperLink ID="linkBalanceTab" runat="server" CssClass="btn btn-primary" NavigateUrl="~/BalanceGeneral.aspx">
                                        Ver Completo
                                    </asp:HyperLink>
                                </div>
                                <asp:GridView ID="gvBalance" runat="server" 
                                    CssClass="grid-view"
                                    AutoGenerateColumns="False" 
                                    DataKeyNames="BalanceID">
                                    <Columns>
                                        <asp:BoundField DataField="BalanceID" HeaderText="ID" />
                                        <asp:BoundField DataField="FechaReporte" HeaderText="Fecha del Reporte" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="Autor" HeaderText="Autor" />
                                        <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:g}" />
                                        <asp:HyperLinkField DataNavigateUrlFields="BalanceID" 
                                            DataNavigateUrlFormatString="~/VerBalance.aspx?ID={0}"
                                            Text="Ver Detalle" 
                                            ControlStyle-CssClass="grid-select-link" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            
                            <!-- Estado de Resultados Tab -->
                            <asp:Panel ID="panelEstadoResultado" runat="server" CssClass="tab-panel">
                                <div class="tab-header">
                                    <h2>Historial de Estados de Resultados</h2>
                                    <asp:HyperLink ID="linkEstadoResultadoTab" runat="server" CssClass="btn btn-primary" NavigateUrl="~/EstadoResultado.aspx">
                                        Ver Completo
                                    </asp:HyperLink>
                                </div>
                                <asp:GridView ID="gvEstadoResultado" runat="server" 
                                    CssClass="grid-view"
                                    AutoGenerateColumns="False" 
                                    DataKeyNames="EstadoResultadoID">
                                    <Columns>
                                        <asp:BoundField DataField="EstadoResultadoID" HeaderText="ID" />
                                        <asp:BoundField DataField="Periodo" HeaderText="Per&iacute;odo" />
                                        <asp:BoundField DataField="Autor" HeaderText="Autor" />
                                        <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:g}" />
                                        <asp:HyperLinkField DataNavigateUrlFields="EstadoResultadoID" 
                                            DataNavigateUrlFormatString="~/VerEstadoResultado.aspx?ID={0}"
                                            Text="Ver Detalle" 
                                            ControlStyle-CssClass="grid-select-link" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            
                            <!-- Estado de Origen Tab -->
                            <asp:Panel ID="panelEstadoOrigen" runat="server" CssClass="tab-panel">
                                <div class="tab-header">
                                    <h2>Historial de Estados de Origen y Aplicaci&oacute;n</h2>
                                    <asp:HyperLink ID="linkEstadoOrigenTab" runat="server" CssClass="btn btn-primary" NavigateUrl="~/EstadoOrigenAplicacion.aspx">
                                        Ver Completo
                                    </asp:HyperLink>
                                </div>
                                <asp:GridView ID="gvEstadoOrigen" runat="server" 
                                    CssClass="grid-view"
                                    AutoGenerateColumns="False" 
                                    DataKeyNames="EstadoFlujoID">
                                    <Columns>
                                        <asp:BoundField DataField="EstadoFlujoID" HeaderText="ID" />
                                        <asp:BoundField DataField="FechaInicio" HeaderText="Fecha de Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="FechaFin" HeaderText="Fecha de Fin" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="Autor" HeaderText="Autor" />
                                        <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:g}" />
                                        <asp:HyperLinkField DataNavigateUrlFields="EstadoFlujoID" 
                                            DataNavigateUrlFormatString="~/VerEstadoOrigen.aspx?ID={0}"
                                            Text="Ver Detalle" 
                                            ControlStyle-CssClass="grid-select-link" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            
                            <!-- Libro Mayor Tab -->
                            <asp:Panel ID="panelLibroMayor" runat="server" CssClass="tab-panel">
                                <div class="tab-header">
                                    <h2>Historial de Asientos (Libro Diario)</h2>
                                    <asp:HyperLink ID="linkLibroMayorTab" runat="server" CssClass="btn btn-primary" NavigateUrl="~/LibroMayor.aspx">
                                        Ver Completo
                                    </asp:HyperLink>
                                </div>
                                <asp:GridView ID="gvLibroMayor" runat="server" 
                                    CssClass="grid-view"
                                    AutoGenerateColumns="False" 
                                    DataKeyNames="AsientoID">
                                    <Columns>
                                        <asp:BoundField DataField="AsientoID" HeaderText="ID" />
                                        <asp:BoundField DataField="FechaAsiento" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="Descripcion" HeaderText="Descripci&oacute;n" />
                                        <asp:BoundField DataField="Autor" HeaderText="Registrado por" />
                                        <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha de Registro" DataFormatString="{0:g}" />
                                        <asp:HyperLinkField DataNavigateUrlFields="AsientoID" 
                                            DataNavigateUrlFormatString="~/VerAsiento.aspx?ID={0}"
                                            Text="Ver Detalle" 
                                            ControlStyle-CssClass="grid-select-link" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            
                            <!-- Reporte Final Tab -->
                            <asp:Panel ID="panelReporteFinal" runat="server" CssClass="tab-panel">
                                <div class="tab-header">
                                    <h2>Reporte Final Completo</h2>
                                    <asp:Button ID="btnExportarReporteFinal" runat="server" Text="Exportar Reporte" 
                                        CssClass="btn btn-success" OnClick="btnExportarReporteFinal_Click" />
                                </div>
                                
                                <div class="reporte-final-container">
                                    <!-- Estadísticas Generales -->
                                    <div class="stats-grid">
                                        <div class="stat-card">
                                            <div class="stat-icon" style="background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);">
                                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                                                    <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
                                                </svg>
                                            </div>
                                            <div class="stat-content">
                                                <h3 class="stat-value" id="statBalances" runat="server">0</h3>
                                                <p class="stat-label">Balances Generales</p>
                                            </div>
                                        </div>
                                        
                                        <div class="stat-card">
                                            <div class="stat-icon" style="background: linear-gradient(135deg, #10b981 0%, #059669 100%);">
                                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                                                    <polyline points="22 6 13.5 14.5 8.5 9.5 2 18"></polyline>
                                                    <polyline points="16 6 22 6 22 12"></polyline>
                                                </svg>
                                            </div>
                                            <div class="stat-content">
                                                <h3 class="stat-value" id="statEstadosResultado" runat="server">0</h3>
                                                <p class="stat-label">Estados de Resultados</p>
                                            </div>
                                        </div>
                                        
                                        <div class="stat-card">
                                            <div class="stat-icon" style="background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);">
                                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                                                    <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                                                </svg>
                                            </div>
                                            <div class="stat-content">
                                                <h3 class="stat-value" id="statEstadosOrigen" runat="server">0</h3>
                                                <p class="stat-label">Estados de Origen</p>
                                            </div>
                                        </div>
                                        
                                        <div class="stat-card">
                                            <div class="stat-icon" style="background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);">
                                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                                                    <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                                    <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                                                </svg>
                                            </div>
                                            <div class="stat-content">
                                                <h3 class="stat-value" id="statAsientos" runat="server">0</h3>
                                                <p class="stat-label">Asientos Contables</p>
                                            </div>
                                        </div>
                                        
                                        <div class="stat-card">
                                            <div class="stat-icon" style="background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);">
                                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2">
                                                    <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
                                                    <line x1="9" y1="3" x2="9" y2="21"></line>
                                                    <line x1="3" y1="9" x2="21" y2="9"></line>
                                                </svg>
                                            </div>
                                            <div class="stat-content">
                                                <h3 class="stat-value" id="statActivos" runat="server">0</h3>
                                                <p class="stat-label">Activos Fijos</p>
                                            </div>
                                        </div>
                                    </div>
                                    
                                    <!-- Resumen por Módulo -->
                                    <div class="reporte-sections">
                                        <!-- Balances Generales -->
                                        <div class="reporte-section">
                                            <div class="reporte-section-header">
                                                <h3>
                                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
                                                    </svg>
                                                    Balances Generales
                                                </h3>
                                                <asp:HyperLink ID="linkVerBalances" runat="server" CssClass="btn-link" NavigateUrl="~/BalanceGeneral.aspx">Ver Todos →</asp:HyperLink>
                                            </div>
                                            <asp:GridView ID="gvReporteBalances" runat="server" 
                                                CssClass="grid-view reporte-grid"
                                                AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:BoundField DataField="BalanceID" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="FechaReporte" HeaderText="Fecha del Reporte" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="Autor" HeaderText="Creado por" />
                                                    <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:HyperLinkField DataNavigateUrlFields="BalanceID" 
                                                        DataNavigateUrlFormatString="~/VerBalance.aspx?ID={0}"
                                                        Text="Ver Detalle" 
                                                        ControlStyle-CssClass="grid-select-link" 
                                                        ItemStyle-CssClass="text-center" 
                                                        HeaderStyle-CssClass="text-center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="empty-state-small">No hay balances registrados</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                        
                                        <!-- Estados de Resultados -->
                                        <div class="reporte-section">
                                            <div class="reporte-section-header">
                                                <h3>
                                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <polyline points="22 6 13.5 14.5 8.5 9.5 2 18"></polyline>
                                                        <polyline points="16 6 22 6 22 12"></polyline>
                                                    </svg>
                                                    Estados de Resultados
                                                </h3>
                                                <asp:HyperLink ID="linkVerEstadosResultado" runat="server" CssClass="btn-link" NavigateUrl="~/EstadoResultado.aspx">Ver Todos →</asp:HyperLink>
                                            </div>
                                            <asp:GridView ID="gvReporteEstadosResultado" runat="server" 
                                                CssClass="grid-view reporte-grid"
                                                AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:BoundField DataField="EstadoResultadoID" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="Periodo" HeaderText="Per&iacute;odo" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="Autor" HeaderText="Creado por" />
                                                    <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:HyperLinkField DataNavigateUrlFields="EstadoResultadoID" 
                                                        DataNavigateUrlFormatString="~/VerEstadoResultado.aspx?ID={0}"
                                                        Text="Ver Detalle" 
                                                        ControlStyle-CssClass="grid-select-link" 
                                                        ItemStyle-CssClass="text-center" 
                                                        HeaderStyle-CssClass="text-center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="empty-state-small">No hay estados de resultados registrados</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                        
                                        <!-- Estados de Origen -->
                                        <div class="reporte-section">
                                            <div class="reporte-section-header">
                                                <h3>
                                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                                                    </svg>
                                                    Estados de Origen y Aplicación
                                                </h3>
                                                <asp:HyperLink ID="linkVerEstadosOrigen" runat="server" CssClass="btn-link" NavigateUrl="~/EstadoOrigenAplicacion.aspx">Ver Todos →</asp:HyperLink>
                                            </div>
                                            <asp:GridView ID="gvReporteEstadosOrigen" runat="server" 
                                                CssClass="grid-view reporte-grid"
                                                AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:BoundField DataField="EstadoFlujoID" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="FechaInicio" HeaderText="Fecha Inicio" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="FechaFin" HeaderText="Fecha Fin" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="Autor" HeaderText="Creado por" />
                                                    <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="empty-state-small">No hay estados de origen registrados</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                        
                                        <!-- Asientos Contables -->
                                        <div class="reporte-section">
                                            <div class="reporte-section-header">
                                                <h3>
                                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                                        <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                                                    </svg>
                                                    Asientos Contables
                                                </h3>
                                                <asp:HyperLink ID="linkVerAsientos" runat="server" CssClass="btn-link" NavigateUrl="~/LibroMayor.aspx">Ver Todos →</asp:HyperLink>
                                            </div>
                                            <asp:GridView ID="gvReporteAsientos" runat="server" 
                                                CssClass="grid-view reporte-grid"
                                                AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:BoundField DataField="AsientoID" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="FechaAsiento" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                                    <asp:BoundField DataField="Autor" HeaderText="Registrado por" />
                                                    <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha de Registro" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:HyperLinkField DataNavigateUrlFields="AsientoID" 
                                                        DataNavigateUrlFormatString="~/VerAsiento.aspx?ID={0}"
                                                        Text="Ver Detalle" 
                                                        ControlStyle-CssClass="grid-select-link" 
                                                        ItemStyle-CssClass="text-center" 
                                                        HeaderStyle-CssClass="text-center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="empty-state-small">No hay asientos registrados</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                        
                                        <!-- Activos Fijos -->
                                        <div class="reporte-section">
                                            <div class="reporte-section-header">
                                                <h3>
                                                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                                        <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
                                                        <line x1="9" y1="3" x2="9" y2="21"></line>
                                                        <line x1="3" y1="9" x2="21" y2="9"></line>
                                                    </svg>
                                                    Activos Fijos
                                                </h3>
                                                <asp:HyperLink ID="linkVerActivos" runat="server" CssClass="btn-link" NavigateUrl="~/LibroMayor.aspx">Ver Todos →</asp:HyperLink>
                                            </div>
                                            <asp:GridView ID="gvReporteActivos" runat="server" 
                                                CssClass="grid-view reporte-grid"
                                                AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:BoundField DataField="ActivoID" HeaderText="ID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="NombreActivo" HeaderText="Nombre del Activo" />
                                                    <asp:BoundField DataField="FechaAdquisicion" HeaderText="Adquirido el" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                    <asp:BoundField DataField="CostoAdquisicion" HeaderText="Costo" DataFormatString="{0:C2}" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right" />
                                                    <asp:BoundField DataField="VidaUtilAnios" HeaderText="Vida &Uacute;til (A&ntilde;os)" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="empty-state-small">No hay activos fijos registrados</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            
                        </div>
                    </div>
                    
                </div>
                
            </main>
            
        </div>
        
    </form>
    
    <script>
        // Modo Oscuro Toggle
        (function() {
            const darkModeToggle = document.getElementById('darkModeToggle');
            const html = document.documentElement;
            
            // Cargar preferencia guardada
            const darkMode = localStorage.getItem('darkMode') === 'true';
            if (darkMode) {
                html.classList.add('dark-mode');
                darkModeToggle.checked = true;
            }
            
            // Manejar cambio del toggle
            darkModeToggle.addEventListener('change', function() {
                if (this.checked) {
                    html.classList.add('dark-mode');
                    localStorage.setItem('darkMode', 'true');
                } else {
                    html.classList.remove('dark-mode');
                    localStorage.setItem('darkMode', 'false');
                }
            });
        })();
    </script>
</body>
</html>
