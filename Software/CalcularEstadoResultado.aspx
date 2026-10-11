<%-- 
  Página: CalcularEstadoResultado.aspx
  Propósito: Formulario para calcular y guardar un nuevo Estado de Resultados.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalcularEstadoResultado.aspx.cs" Inherits="Software.CalcularEstadoResultado" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Calcular Estado de Resultados</title>
    <link href="Styles/Contenido.css?v=3.9" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');
        
        .reporte-container {
            border: 2px solid #e8eef3;
            border-radius: 16px;
            padding: 2.5rem 3rem;
            margin-top: 2rem;
            background: linear-gradient(135deg, #ffffff 0%, #f8f9fb 100%);
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
        }
        .reporte-container h3 {
            color: #1a4d8c;
            border-bottom: 3px solid #2c5aa0;
            padding-bottom: 1rem;
            margin-bottom: 1.75rem;
            font-size: 1.5rem;
            font-weight: 700;
            letter-spacing: -0.3px;
            line-height: 1.3;
            font-family: 'Inter', sans-serif;
        }
        .reporte-container .linea-item {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e8eef3;
            transition: background-color 0.2s ease;
        }
        .reporte-container .linea-item:hover {
            background-color: #f8f9fb;
            padding-left: 8px;
            padding-right: 8px;
            margin-left: -8px;
            margin-right: -8px;
            border-radius: 6px;
        }
        .reporte-container .linea-item span:first-child {
            color: #1a1a1a;
            font-weight: 500;
            font-family: 'Inter', sans-serif;
            font-size: 0.9375rem;
            letter-spacing: -0.01em;
        }
        .reporte-container .linea-item span:last-child {
            color: #1a4d8c;
            font-weight: 600;
            font-family: 'Inter', 'Courier New', monospace;
            font-size: 0.9375rem;
            letter-spacing: 0.02em;
        }
        .reporte-container .linea-total {
            display: flex;
            justify-content: space-between;
            padding: 18px 16px;
            margin-top: 16px;
            font-weight: 700;
            font-size: 1.125rem;
            border-top: 3px solid #2c5aa0;
            background: linear-gradient(135deg, #e8f4fd 0%, #d4e8fa 100%);
            border-radius: 12px;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.02em;
        }
        .reporte-container .linea-total span:first-child {
            color: #1a4d8c;
            font-weight: 700;
        }
        .reporte-container .linea-total span:last-child {
            color: #1a4d8c;
            font-family: 'Inter', 'Courier New', monospace;
            font-size: 1.1875rem;
            font-weight: 800;
            letter-spacing: 0.03em;
        }
        .reporte-container .utilidad-ok {
            color: #155724;
            font-weight: 700;
            text-align: center;
            margin-top: 2rem;
            padding: 1.25rem 1.5rem;
            background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
            border-radius: 12px;
            border-left: 5px solid #28a745;
            font-size: 1.0625rem;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            line-height: 1.5;
        }
        .date-range {
            display: flex;
            gap: 1.5rem;
            align-items: flex-end;
            margin-bottom: 2rem;
            padding: 1.5rem;
            background: linear-gradient(135deg, #f8f9fb 0%, #ffffff 100%);
            border-radius: 12px;
            border: 1px solid #e8eef3;
        }
        .date-range > div {
            flex: 1;
        }
        .date-range label {
            font-weight: 600;
            display: block;
            margin-bottom: 0.875rem;
            color: #1a1a1a;
            font-size: 0.9375rem;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            text-align: left;
        }
        
        .date-range > div {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }
        
        .reporte-container > div[style*="grid"] > div {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }
        
        .reporte-container label {
            text-align: left;
            display: block;
            margin-bottom: 0;
        }
        .form-control {
            padding: 14px 18px;
            border: 2px solid #e8eef3;
            border-radius: 12px;
            transition: all 0.3s ease;
            font-family: 'Inter', sans-serif;
            font-size: 0.9375rem;
            font-weight: 400;
            letter-spacing: -0.01em;
            line-height: 1.5;
            color: #1a1a1a;
        }
        .form-control:focus {
            border-color: #2c5aa0;
            box-shadow: 0 0 0 4px rgba(44, 90, 160, 0.1);
            outline: none;
            font-weight: 500;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Calcular Nuevo Estado de Resultados</h1>
                    <p class="header-subtitle">Ingresa los datos financieros del período para generar el análisis</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/EstadoResultado.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver
                </asp:HyperLink>
            </div>

            <div class="date-range">
                <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                    <asp:Label ID="lblPeriodo" runat="server" Text="Per&iacute;odo:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                    <asp:TextBox ID="txtPeriodo" runat="server" CssClass="form-control" Width="100%" placeholder="Ej: 2024, Enero 2024, Q1 2024"></asp:TextBox>
                </div>
            </div>

            <div class="reporte-container" style="margin-top: 2rem;">
                <h3>Datos del Estado de Resultados</h3>
                
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; margin-top: 1.5rem;">
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblIngresosVentas" runat="server" Text="Ingresos por ventas:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtIngresosVentas" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="0"></asp:TextBox>
                    </div>
                    
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblCostoBienesVendidos" runat="server" Text="Costo de los bienes vendidos:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtCostoBienesVendidos" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="0"></asp:TextBox>
                    </div>
                    
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblGastosOperativos" runat="server" Text="Gastos operativos:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtGastosOperativos" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="0"></asp:TextBox>
                    </div>
                    
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblGastosIntereses" runat="server" Text="Gastos por intereses:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtGastosIntereses" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="0"></asp:TextBox>
                    </div>
                    
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblTasaImpuestos" runat="server" Text="Tasa de impuestos (%):" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtTasaImpuestos" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="40" placeholder="Ej: 40"></asp:TextBox>
                    </div>
                    
                    <div style="display: flex; flex-direction: column; gap: 0.5rem;">
                        <asp:Label ID="lblDividendos" runat="server" Text="Dividendos en efectivo:" Font-Bold="true" style="text-align: left; display: block;"></asp:Label>
                        <asp:TextBox ID="txtDividendos" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Width="100%" Text="0"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="action-bar">
                <asp:Button ID="btnCalcular" runat="server" Text="Calcular Estado de Resultados" 
                    CssClass="btn btn-primary" OnClick="btnCalcular_Click" />
                
                <asp:Button ID="btnGuardarReporte" runat="server" Text="Guardar Reporte" 
                    CssClass="btn btn-success" OnClick="btnGuardarReporte_Click" 
                    Visible="false" /> 
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <asp:Literal ID="ltlEstadoReporte" runat="server"></asp:Literal>

        </div>
    </form>
</body>
</html>

