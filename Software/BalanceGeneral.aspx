<%-- 
  Página: BalanceGeneral.aspx
  Propósito: Muestra historial de Balances y permite crear/exportar.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BalanceGeneral.aspx.cs" Inherits="Software.BalanceGeneral" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Balance General | Sistema Financiero</title>
    <link href="Styles/Contenido.css?v=3.8" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Balance General</h1>
                    <p class="header-subtitle">Gestiona y visualiza tus balances contables de forma profesional</p>
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
                            <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                        </svg>
                        Balances Generales Registrados
                    </h2>
                    <asp:Button ID="btnNuevoBalance" runat="server" Text="Nuevo Balance" 
                        CssClass="btn btn-primary btn-icon" OnClick="btnNuevoBalance_Click" />
                </div>

                <div class="table-wrapper">
                    <asp:GridView ID="gvBalances" runat="server" 
                        CssClass="grid-view"
                        AutoGenerateColumns="False" 
                        DataKeyNames="BalanceID" 
                        OnSelectedIndexChanged="gvBalances_SelectedIndexChanged"
                        OnRowDataBound="gvBalances_RowDataBound">
                        
                        <Columns>
                            <asp:BoundField DataField="BalanceID" HeaderText="ID" SortExpression="BalanceID" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:TemplateField HeaderText="Fecha del Reporte" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                <ItemTemplate>
                                    <asp:Label ID="lblFechaReporte" runat="server" Text='<%# Eval("FechaReporte", "{0:dd/MM/yyyy}") %>' Visible='<%# !Convert.ToBoolean(ViewState["EditMode_" + Eval("BalanceID")]) %>'></asp:Label>
                                    <asp:TextBox ID="txtFechaReporte" runat="server" TextMode="Date" Text='<%# Eval("FechaReporte", "{0:yyyy-MM-dd}") %>' 
                                        Visible='<%# Convert.ToBoolean(ViewState["EditMode_" + Eval("BalanceID")]) %>' CssClass="form-control" style="width: 150px; display: inline-block;"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Autor" HeaderText="Creado por" SortExpression="Autor" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            <asp:BoundField DataField="FechaGeneracion" HeaderText="Fecha de Generaci&oacute;n" SortExpression="FechaGeneracion" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                            
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditarFecha" runat="server" CommandArgument='<%# Eval("BalanceID") %>' 
                                        OnCommand="btnEditarFecha_Command" CssClass="grid-select-link" 
                                        Visible='<%# !Convert.ToBoolean(ViewState["EditMode_" + Eval("BalanceID")]) %>'>
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnGuardarFecha" runat="server" CommandArgument='<%# Eval("BalanceID") %>' 
                                        OnCommand="btnGuardarFecha_Command" CssClass="grid-select-link" 
                                        Visible='<%# Convert.ToBoolean(ViewState["EditMode_" + Eval("BalanceID")]) %>' style="color: #10b981;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><path d="M20 6L9 17l-5-5"/></svg>Guardar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnCancelarFecha" runat="server" CommandArgument='<%# Eval("BalanceID") %>' 
                                        OnCommand="btnCancelarFecha_Command" CssClass="grid-select-link" 
                                        Visible='<%# Convert.ToBoolean(ViewState["EditMode_" + Eval("BalanceID")]) %>' style="color: #dc2626; margin-left: 5px;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>Cancelar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandArgument='<%# Eval("BalanceID") %>' 
                                        OnCommand="btnEliminar_Command" CssClass="grid-select-link" 
                                        OnClientClick="return confirm('¿Está seguro de que desea eliminar este balance? Esta acción no se puede deshacer.');"
                                        style="color: #dc2626; margin-left: 5px;">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.25em;"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:CommandField ShowSelectButton="True" SelectText="Seleccionar" 
                                ControlStyle-CssClass="grid-select-link" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                        </Columns>

                        <SelectedRowStyle CssClass="grid-view-selected-row" />
                        <EmptyDataTemplate>
                            <div class="empty-state">
                                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                    <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                                </svg>
                                <p>No hay balances registrados</p>
                                <p class="empty-subtitle">Calcula tu primer balance para comenzar</p>
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