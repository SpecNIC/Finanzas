<%-- 
  Página: CalcularBalance.aspx
  Propósito: Formulario para calcular y guardar un nuevo Balance General.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalcularBalance.aspx.cs" Inherits="Software.CalcularBalance" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Calcular Balance General</title>
    <link href="Styles/Contenido.css?v=3.0" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    
    <style>
        /* Estilos específicos solo para esta página */
        .content-container {
            max-width: 1200px !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Calcular Nuevo Balance</h1>
                    <p class="header-subtitle">Selecciona la fecha para generar tu balance general contable</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/BalanceGeneral.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver
                </asp:HyperLink>
            </div>

            <div class="form-section">
                <div class="date-input-container">
                    <label for="<%=txtFechaReporte.ClientID%>"><svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="vertical-align: middle; margin-right: 0.5em;"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>Fecha del Reporte</label>
                    <div class="date-input-wrapper">
                        <asp:TextBox ID="txtFechaReporte" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                </div>

                <div class="action-bar">
                    <asp:Button ID="btnCalcular" runat="server" Text="Calcular Balance" 
                        CssClass="btn btn-primary" OnClick="btnCalcular_Click" />
                    
                    <asp:Button ID="btnGuardarReporte" runat="server" Text="Guardar Reporte" 
                        CssClass="btn btn-success" OnClick="btnGuardarReporte_Click" 
                        Visible="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>
            </div>

            <asp:Literal ID="ltlBalanceReporte" runat="server"></asp:Literal>

        </div>
    </form>
</body>
</html>