<%-- 
  Página: CalcularEstadoOrigen.aspx
  Propósito: Compara dos Balances Generales YA GUARDADOS.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalcularEstadoOrigen.aspx.cs" Inherits="Software.CalcularEstadoOrigen" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Calcular Estado de Origen y Aplicación</title>
    <link href="Styles/Contenido.css?v=2.3" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    
    <style>
        .reporte-container {
            border: 2px solid #e8eef3;
            border-radius: 16px;
            padding: 2rem 1.5rem;
            margin-top: 2rem;
            background: linear-gradient(135deg, #ffffff 0%, #f8f9fb 100%);
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 2rem;
        }
        .reporte-columna {
            padding: 0 1.5rem;
        }
        .reporte-columna:first-child {
            border-right: 3px solid #2c5aa0;
            padding-right: 2rem;
        }
        .reporte-columna:last-child {
            padding-left: 2rem;
        }
        .reporte-columna h3 {
            color: #1a4d8c;
            border-bottom: 3px solid #2c5aa0;
            padding-bottom: 12px;
            margin-bottom: 1.5rem;
            font-size: 1.4rem;
            font-weight: 700;
            letter-spacing: -0.3px;
        }
        .reporte-columna .linea-item {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e8eef3;
            transition: background-color 0.2s ease;
        }
        .reporte-columna .linea-item:hover {
            background-color: #f8f9fb;
            padding-left: 8px;
            padding-right: 8px;
            margin-left: -8px;
            margin-right: -8px;
            border-radius: 6px;
        }
        .reporte-columna .linea-item span:first-child {
            color: #2c3e50;
            font-weight: 500;
        }
        .reporte-columna .linea-item span:last-child {
            color: #1a4d8c;
            font-weight: 600;
            font-family: 'Courier New', monospace;
        }
        .reporte-columna .linea-total {
            display: flex;
            justify-content: space-between;
            padding: 16px 12px;
            margin-top: 12px;
            font-weight: 700;
            font-size: 1.15rem;
            border-top: 3px solid #2c5aa0;
            background: linear-gradient(135deg, #e8f4fd 0%, #d4e8fa 100%);
            border-radius: 8px;
        }
        .reporte-columna .linea-total span:first-child {
            color: #1a4d8c;
        }
        .reporte-columna .linea-total span:last-child {
            color: #1a4d8c;
            font-family: 'Courier New', monospace;
            font-size: 1.2rem;
        }
        .reporte-balance-ok {
            grid-column: 1 / -1;
            color: #155724;
            font-weight: 700;
            text-align: center;
            margin-top: 2rem;
            padding: 1.5rem;
            border-top: 3px solid #e8eef3;
            background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
            border-radius: 10px;
            border-left: 5px solid #28a745;
            font-size: 1.1rem;
        }
        select.form-control {
            padding: 12px 16px;
            border: 2px solid #e8eef3;
            border-radius: 10px;
            transition: all 0.3s ease;
        }
        select.form-control:focus {
            border-color: #2c5aa0;
            box-shadow: 0 0 0 4px rgba(44, 90, 160, 0.1);
            outline: none;
        }
        .balance-selector-container {
            display: flex;
            gap: 2rem;
            margin-bottom: 1.5rem;
            background: linear-gradient(135deg, #f8f9fb 0%, #ffffff 100%);
            padding: 1.5rem;
            border-radius: 12px;
            border: 1px solid #e8eef3;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        }
        .balance-selector-item {
            flex: 1;
        }
        .balance-selector-item label {
            display: block;
            margin-bottom: 0.75rem;
            font-weight: 600;
            color: #2c3e50;
            font-size: 0.95rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Calcular Estado de Origen y Aplicación</h1>
                    <p class="header-subtitle">Compara dos balances para analizar el flujo de efectivo y origen de fondos</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/EstadoOrigenAplicacion.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver
                </asp:HyperLink>
            </div>

            <!-- === ÁREA DE ENTRADA MODIFICADA === -->
            <div class="balance-selector-container">
                <div class="balance-selector-item">
                    <asp:Label ID="lblBalanceInicial" runat="server" Text="Seleccione Balance Inicial (Ej. Año Anterior):" AssociatedControlID="ddlBalanceInicial"></asp:Label>
                    <asp:DropDownList ID="ddlBalanceInicial" runat="server" CssClass="form-control" Width="100%"></asp:DropDownList>
                </div>
                <div class="balance-selector-item">
                    <asp:Label ID="lblBalanceFinal" runat="server" Text="Seleccione Balance Final (Ej. Año Actual):" AssociatedControlID="ddlBalanceFinal"></asp:Label>
                    <asp:DropDownList ID="ddlBalanceFinal" runat="server" CssClass="form-control" Width="100%"></asp:DropDownList>
                </div>
            </div>
            <!-- === FIN DE ÁREA MODIFICADA === -->

            <div class="action-bar">
                <asp:Button ID="btnCalcular" runat="server" Text="Calcular Estado" 
                    CssClass="btn btn-primary" OnClick="btnCalcular_Click" />
                
                <asp:Button ID="btnGuardarReporte" runat="server" Text="Guardar Reporte" 
                    CssClass="btn btn-success" OnClick="btnGuardarReporte_Click" 
                    Visible="false" /> 
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <asp:Literal ID="ltlReporte" runat="server"></asp:Literal>

        </div>
    </form>
</body>
</html>