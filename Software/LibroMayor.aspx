<%-- 
  Página: LibroMayor.aspx
  Propósito: Muestra el historial de asientos (Libro Diario) y Activos Fijos.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LibroMayor.aspx.cs" Inherits="Software.LibroMayor" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Libro Mayor y Activos | Sistema Financiero</title>
    <link href="Styles/Contenido.css?v=3.8" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Libro Mayor y Activos Fijos</h1>
                    <p class="header-subtitle">Gestión completa de asientos contables y activos fijos</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/Menu.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver al Men&uacute;
                </asp:HyperLink>
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <!-- Sección: Libro Diario -->
            <div class="section-card">
                <div class="section-header">
                    <h2 class="section-title">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/>
                            <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/>
                        </svg>
                        Asientos Contables Registrados
                    </h2>
                    <asp:Button ID="btnNuevoAsiento" runat="server" Text="Nuevo Asiento" 
                        CssClass="btn btn-primary btn-icon" OnClick="btnNuevoAsiento_Click" />
                </div>

                <div class="table-wrapper">
                    <asp:GridView ID="gvAsientos" runat="server" 
                        CssClass="grid-view"
                        AutoGenerateColumns="False" 
                        DataKeyNames="AsientoID"
                        OnRowDataBound="gvAsientos_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="AsientoID" HeaderText="ID" SortExpression="AsientoID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaAsiento" HeaderText="Fecha" SortExpression="FechaAsiento" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" SortExpression="Descripcion" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="Autor" HeaderText="Registrado por" SortExpression="Autor" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha de Registro" SortExpression="FechaRegistro" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            
                            <asp:HyperLinkField DataNavigateUrlFields="AsientoID" 
                                DataNavigateUrlFormatString="~/VerAsiento.aspx?ID={0}"
                                Text="Ver Detalle" 
                                ControlStyle-CssClass="grid-select-link" 
                                ItemStyle-CssClass="text-center" 
                                HeaderStyle-CssClass="text-center" />
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                               
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEliminarAsiento" runat="server" CommandArgument='<%# Eval("AsientoID") %>' 
                                        OnCommand="btnEliminarAsiento_Command" CssClass="grid-select-link" 
                                        OnClientClick="return confirm('¿Está seguro de que desea eliminar este asiento? Esta acción no se puede deshacer.');"
                                        style="color: #dc2626;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg> 
                                    </asp:LinkButton>
                                </ItemTemplate>                             
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-state">   
                                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/>
                                    <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/>
                                </svg>
                                <p>No hay asientos registrados</p>
                                <p class="empty-subtitle">Registra tu primer asiento contable para comenzar</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Sección: Activos Fijos -->
            <div class="section-card section-spacing">
                <div class="section-header">
                    <h2 class="section-title">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="3" y="3" width="18" height="18" rx="2" ry="2"/>
                            <line x1="9" y1="3" x2="9" y2="21"/>
                            <line x1="3" y1="9" x2="21" y2="9"/>
                        </svg>
                        Gestión de Activos Fijos
                    </h2>
                    <asp:Button ID="btnNuevoActivo" runat="server" Text="Nuevo Activo" 
                        CssClass="btn btn-primary btn-icon" OnClick="btnNuevoActivo_Click" />
                </div>

                <div class="table-wrapper">
                    <asp:GridView ID="gvActivosFijos" runat="server" 
                        CssClass="grid-view"
                        AutoGenerateColumns="False" 
                        DataKeyNames="ActivoID"
                        OnRowDataBound="gvActivosFijos_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="ActivoID" HeaderText="ID" SortExpression="ActivoID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="NombreActivo" HeaderText="Nombre del Activo" SortExpression="NombreActivo" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaAdquisicion" HeaderText="Adquirido el" SortExpression="FechaAdquisicion" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="CostoAdquisicion" HeaderText="Costo" SortExpression="CostoAdquisicion" DataFormatString="{0:C2}" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right" />
                            <asp:BoundField DataField="VidaUtilAnios" HeaderText="Vida &Uacute;til (A&ntilde;os)" SortExpression="VidaUtilAnios" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEliminarActivo" runat="server" CommandArgument='<%# Eval("ActivoID") %>' 
                                        OnCommand="btnEliminarActivo_Command" CssClass="grid-select-link" 
                                        OnClientClick="return confirm('¿Está seguro de que desea eliminar este activo fijo? Esta acción no se puede deshacer.');"
                                        style="color: #dc2626;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-state">
                                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <rect x="3" y="3" width="18" height="18" rx="2" ry="2"/>
                                </svg>
                                <p>No hay activos fijos registrados</p>
                                <p class="empty-subtitle">Registra tu primer activo fijo para comenzar</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

        </div>
    </form>
</body>
</html>