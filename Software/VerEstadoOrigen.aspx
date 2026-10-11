<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerEstadoOrigen.aspx.cs" Inherits="Software.VerEstadoOrigen" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Detalle de Estado de Origen y Aplicaci&oacute;n</title>
    <link href="Styles/Contenido.css?v=2.3" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        body {
            color: #2c3e50;
            background-color: #f5f7fa;
        }
        .reporte-container {
            border: 2px solid #e8eef3;
            border-radius: 16px;
            padding: 2.5rem;
            background: linear-gradient(135deg, #ffffff 0%, #f8f9fb 100%);
            margin-top: 1.5rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
            color: #2c3e50;
        }
        .reporte-header {
            text-align: center;
            margin-bottom: 2.5rem;
            padding-bottom: 1.5rem;
            border-bottom: 3px solid #2c5aa0;
            background: linear-gradient(135deg, #e8f4fd 0%, #d4e8fa 100%);
            padding: 1.5rem;
            border-radius: 12px;
        }
        .reporte-header h2 {
            color: #1a4d8c;
            font-size: 1.8rem;
            font-weight: 700;
            margin: 0 0 0.5rem 0;
            letter-spacing: -0.5px;
        }
        .reporte-header p {
            color: #5a6c7d;
            font-size: 1rem;
            margin: 0;
            font-weight: 500;
        }
        .categoria-titulo {
            color: #1a4d8c;
            font-size: 1.3rem;
            margin-top: 2rem;
            margin-bottom: 1rem;
            padding-bottom: 0.75rem;
            border-bottom: 2px solid #2c5aa0;
            font-weight: 700;
            letter-spacing: -0.3px;
        }
        .fila-detalle {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 16px;
            border-bottom: 1px solid #e8eef3;
            transition: background-color 0.2s ease;
        }
        .fila-detalle:hover {
            background-color: #f8f9fb;
            border-radius: 6px;
        }
        .fila-detalle span:first-child {
            color: #2c3e50;
            font-weight: 500;
        }
        .fila-detalle .monto-valor {
            color: #1a4d8c;
            font-weight: 600;
            font-family: 'Courier New', monospace;
            margin-left: auto;
        }
        .fila-total {
            display: flex;
            justify-content: space-between;
            padding: 16px 16px;
            font-weight: 700;
            font-size: 1.15rem;
            background: linear-gradient(135deg, #e8f4fd 0%, #d4e8fa 100%);
            margin-top: 8px;
            border-radius: 8px;
            border-top: 3px solid #2c5aa0;
        }
        .fila-total span:first-child {
            color: #1a4d8c;
        }
        .fila-total span:last-child {
            color: #1a4d8c;
            font-family: 'Courier New', monospace;
            font-size: 1.2rem;
        }
        .estadisticas-section {
            margin: 2.5rem 0;
            padding: 2rem;
            background: linear-gradient(135deg, #f8f9fb 0%, #ffffff 100%);
            border-radius: 12px;
            border: 1px solid #e2e8f0;
        }
        .seccion-titulo {
            color: #1a4d8c;
            font-size: 1.4rem;
            font-weight: 700;
            margin-bottom: 1.5rem;
            padding-bottom: 0.75rem;
            border-bottom: 2px solid #2c5aa0;
        }
        .estadisticas-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1.25rem;
        }
        .estadistica-card {
            background: #ffffff;
            padding: 1.5rem;
            border-radius: 10px;
            border: 2px solid #e2e8f0;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            transition: all 0.3s ease;
            color: #2c3e50;
        }
        .estadistica-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 4px 16px rgba(44, 90, 160, 0.15);
            border-color: #2c5aa0;
        }
        .estadistica-card.total {
            background: linear-gradient(135deg, #2c5aa0 0%, #1e3a5f 100%);
            color: #ffffff !important;
            border-color: #2c5aa0;
        }
        .estadistica-card.total * {
            color: #ffffff !important;
        }
        .estadistica-label {
            font-size: 0.9375rem;
            color: #64748b;
            font-weight: 600;
            margin-bottom: 0.5rem;
        }
        .estadistica-card.total .estadistica-label {
            color: rgba(255, 255, 255, 0.9);
        }
        .estadistica-cantidad {
            font-size: 1.75rem;
            font-weight: 800;
            color: #2c5aa0;
            margin-bottom: 0.5rem;
        }
        .estadistica-card.total .estadistica-cantidad {
            color: #ffffff;
        }
        .estadistica-monto {
            font-size: 1.125rem;
            font-weight: 700;
            color: #1a4d8c;
            font-family: 'Courier New', monospace;
        }
        .estadistica-card.total .estadistica-monto {
            color: #ffffff !important;
        }
        .balance-status {
            text-align: center;
            padding: 1.5rem;
            border-radius: 10px;
            margin-top: 1.5rem;
            font-size: 1.125rem;
        }
        .balance-status.success {
            background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
            color: #065f46;
            border: 2px solid #10b981;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content-container">
            
            <div class="content-header">
                <h1>Visualizar Estado de Origen y Aplicaci&oacute;n</h1>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/EstadoOrigenAplicacion.aspx">&larr; Volver al Historial</asp:HyperLink>
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <asp:Literal ID="ltlReporte" runat="server"></asp:Literal>

        </div>
    </form>
</body>
</html>

