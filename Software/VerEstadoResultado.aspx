<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerEstadoResultado.aspx.cs" Inherits="Software.VerEstadoResultado" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Detalle de Estado de Resultados</title>
    <link href="Styles/Contenido.css?v=3.4" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');
        
        .reporte-container {
            border: 1px solid #e2e8f0;
            border-radius: 20px;
            padding: 0;
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            margin: 0 3rem 2.5rem 3rem;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            position: relative;
        }
        
        .reporte-container::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        .reporte-header {
            text-align: center;
            margin: 0;
            padding: 3rem 3rem 2.5rem 3rem;
            background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
            border-bottom: 2px solid #e2e8f0;
            position: relative;
        }
        
        .reporte-header::after {
            content: '';
            position: absolute;
            bottom: 0;
            left: 50%;
            transform: translateX(-50%);
            width: 80px;
            height: 2px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        .reporte-header h2 {
            color: #1a4d8c;
            font-size: 2rem;
            font-weight: 800;
            margin: 0 0 1rem 0;
            letter-spacing: -0.8px;
            font-family: 'Inter', sans-serif;
        }
        
        .reporte-header p {
            color: #475569;
            font-size: 1rem;
            margin: 0.5rem 0;
            font-weight: 500;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
        }
        
        .reporte-body {
            padding: 2.5rem 3rem;
        }
        
        .linea-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1rem 1.5rem;
            border-bottom: 1px solid #f1f5f9;
            transition: all 0.2s ease;
            font-family: 'Inter', sans-serif;
        }
        
        .linea-item:hover {
            background-color: #f8fafc;
            transform: translateX(4px);
            border-left: 3px solid #e2e8f0;
            margin-left: -3px;
        }
        
        .linea-item span:first-child {
            color: #334155;
            font-weight: 500;
            font-size: 0.9375rem;
            letter-spacing: -0.01em;
            flex: 1;
            padding-right: 2rem;
        }
        
        .linea-item span:last-child {
            color: #1a1a1a;
            font-weight: 600;
            font-family: 'Inter', 'Courier New', monospace;
            font-size: 1rem;
            text-align: right;
            min-width: 140px;
            letter-spacing: 0.02em;
        }
        
        .linea-item span.negative {
            color: #dc2626;
        }
        
        .linea-total {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1.25rem 1.5rem;
            font-weight: 700;
            font-size: 1.0625rem;
            background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
            margin-top: 0.5rem;
            border-radius: 8px;
            border-left: 4px solid #2c5aa0;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
        }
        
        .linea-total span:first-child {
            color: #1a4d8c;
            font-size: 1.0625rem;
            font-weight: 700;
            flex: 1;
            padding-right: 2rem;
        }
        
        .linea-total span:last-child {
            color: #1a4d8c;
            font-family: 'Inter', 'Courier New', monospace;
            font-size: 1.25rem;
            font-weight: 700;
            text-align: right;
            min-width: 140px;
            letter-spacing: 0.02em;
        }
        
        .linea-total span.negative {
            color: #dc2626;
        }
        
        .linea-final {
            background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%) !important;
            border-left: 4px solid #059669 !important;
            border-top: 2px solid #86efac;
            margin-top: 1rem !important;
            padding: 1.5rem 1.5rem !important;
            box-shadow: 0 4px 12px rgba(5, 150, 105, 0.15);
        }
        
        .linea-final.negative {
            background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%) !important;
            border-left: 4px solid #dc2626 !important;
            border-top: 2px solid #fecaca;
            box-shadow: 0 4px 12px rgba(220, 38, 38, 0.15);
        }
        
        .linea-final span:first-child {
            color: #059669 !important;
            font-size: 1.125rem !important;
        }
        
        .linea-final.negative span:first-child {
            color: #dc2626 !important;
        }
        
        .linea-final span:last-child {
            color: #059669 !important;
            font-size: 1.375rem !important;
            font-weight: 800 !important;
        }
        
        .linea-final.negative span:last-child {
            color: #dc2626 !important;
        }
        
        .utilidad-ok {
            margin-top: 2rem;
            padding: 1.25rem 1.5rem;
            border-radius: 12px;
            font-weight: 600;
            font-size: 0.9375rem;
            text-align: center;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
            color: #059669;
            border: 2px solid #86efac;
            box-shadow: 0 2px 8px rgba(5, 150, 105, 0.15);
        }
        
        .utilidad-ok[style*="background: linear-gradient"] {
            background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%) !important;
            color: #991b1b !important;
            border: 2px solid #fecaca !important;
            box-shadow: 0 2px 8px rgba(220, 38, 38, 0.15) !important;
        }
        
        @media (max-width: 768px) {
            .reporte-container {
                margin: 0 1.5rem 2rem 1.5rem !important;
            }
            
            .reporte-header {
                padding: 2rem 1.5rem !important;
            }
            
            .reporte-body {
                padding: 2rem 1.5rem !important;
            }
            
            .linea-item,
            .linea-total,
            .linea-final {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
            }
            
            .linea-item span:last-child,
            .linea-total span:last-child,
            .linea-final span:last-child {
                text-align: left;
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content-container">
            
            <div class="content-header">
                <h1>| Visualizar Estado de Resultados</h1>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/EstadoResultado.aspx">&mdash; Volver al Historial</asp:HyperLink>
            </div>

            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <asp:Literal ID="ltlReporte" runat="server"></asp:Literal>

        </div>
    </form>
</body>
</html>

