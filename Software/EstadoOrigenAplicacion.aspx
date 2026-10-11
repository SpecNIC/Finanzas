<%-- 
  Página: EstadoOrigenAplicacion.aspx
  Propósito: Muestra historial de Estados de Origen y Aplicación.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EstadoOrigenAplicacion.aspx.cs" Inherits="Software.EstadoOrigenAplicacion" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Estado de Origen y Aplicación | Sistema Financiero</title>
    <link href="Styles/Contenido.css?v=3.8" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Estado de Origen y Aplicación</h1>
                    <p class="header-subtitle">Análisis del flujo de efectivo y movimientos financieros</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/Menu.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver al Men&uacute;
                </asp:HyperLink>
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <div class="section-card">
                <div class="section-header">
                    <h2 class="section-title">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/>
                            <polyline points="3.27 6.96 12 12.01 20.73 6.96"/>
                            <line x1="12" y1="22.08" x2="12" y2="12"/>
                        </svg>
                        Estados de Flujo de Efectivo Registrados
                    </h2>
                    <asp:Button ID="btnNuevoEstado" runat="server" Text="Nuevo Estado" 
                        CssClass="btn btn-primary btn-icon" OnClick="btnNuevoEstado_Click" />
                </div>

                <div class="table-wrapper">
                    <asp:GridView ID="gvEstadosOrigen" runat="server" 
                        CssClass="grid-view"
                        AutoGenerateColumns="False" 
                        DataKeyNames="EstadoFlujoID" 
                        OnSelectedIndexChanged="gvEstadosOrigen_SelectedIndexChanged"
                        OnRowDataBound="gvEstadosOrigen_RowDataBound">
                        
                        <Columns>
                            <asp:BoundField DataField="EstadoFlujoID" HeaderText="ID" SortExpression="EstadoFlujoID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaInicio" HeaderText="Fecha de Inicio" SortExpression="FechaInicio" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaFin" HeaderText="Fecha de Fin" SortExpression="FechaFin" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="Autor" HeaderText="Creado por" SortExpression="Autor" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" SortExpression="FechaGeneracion" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            
                            <asp:CommandField ShowSelectButton="True" SelectText="Seleccionar" 
                                ControlStyle-CssClass="grid-select-link" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandArgument='<%# Eval("EstadoFlujoID") %>' 
                                        OnCommand="btnEliminar_Command" CssClass="grid-select-link" 
                                        OnClientClick="return confirm('¿Está seguro de que desea eliminar este estado de origen y aplicación? Esta acción no se puede deshacer.');"
                                        style="color: #dc2626;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <SelectedRowStyle CssClass="grid-view-selected-row" />
                        <EmptyDataTemplate>
                            <div class="empty-state">
                                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/>
                                </svg>
                                <p>No hay estados de flujo registrados</p>
                                <p class="empty-subtitle">Crea tu primer estado para comenzar</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

                <div class="action-bar-footer">
                    <asp:Button ID="btnVerDetalle" runat="server" Text="Ver Detalle" 
                        CssClass="btn btn-info" OnClick="btnVerDetalle_Click" 
                        Enabled="false" /> 
                    <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" 
                        CssClass="btn btn-success" OnClick="btnExportarExcel_Click" 
                        Enabled="false" /> 
                </div>
            </div>

        </div>
    </form>
</body>
</html>