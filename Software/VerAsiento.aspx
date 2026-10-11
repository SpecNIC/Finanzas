<%-- 
  Página: VerAsiento.aspx
  Propósito: Muestra el detalle (Debe/Haber) de un asiento contable específico.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerAsiento.aspx.cs" Inherits="Software.VerAsiento" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Detalle de Asiento</title>
    <link href="Styles/Contenido.css?v=3.4" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');
        
        .asiento-header {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 2.5rem 3rem;
            margin: 0 3rem 2.5rem 3rem;
            display: grid;
            grid-template-columns: 1fr 2fr;
            gap: 2rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
            position: relative;
            overflow: hidden;
        }
        
        .asiento-header::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        .asiento-header > div {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }
        
        .asiento-header strong {
            color: #64748b;
            display: block;
            margin-bottom: 0;
            font-weight: 600;
            font-size: 0.8125rem;
            letter-spacing: 0.5px;
            text-transform: uppercase;
            font-family: 'Inter', sans-serif;
        }
        
        .asiento-header span,
        .asiento-header label {
            color: #1a1a1a;
            font-size: 1.125rem;
            font-weight: 600;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            line-height: 1.5;
        }
        
        .asiento-header .desc {
            grid-column: 1 / -1;
            padding-top: 1.5rem;
            margin-top: 1.5rem;
            border-top: 1px solid #f1f5f9;
        }
        
        .grid-view {
            margin: 0 3rem !important;
        }
        
        .totals-footer {
            margin: 2.5rem 3rem;
            padding: 0;
            background: transparent;
            border-radius: 16px;
            box-shadow: none;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            position: relative;
        }
        
        .totals-inner {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 2rem 2.5rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
            position: relative;
            overflow: hidden;
        }
        
        .totals-inner::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        .totals-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1rem 0;
            border-bottom: 1px solid #f1f5f9;
        }
        
        .totals-row:last-of-type {
            border-bottom: none;
            padding-bottom: 0;
        }
        
        .totals-label {
            font-size: 0.9375rem;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            font-family: 'Inter', sans-serif;
        }
        
        .totals-value {
            font-size: 1.375rem;
            font-weight: 700;
            font-family: 'Inter', 'Courier New', monospace;
            letter-spacing: -0.02em;
            color: #059669;
        }
        
        @media (max-width: 768px) {
            .asiento-header {
                grid-template-columns: 1fr;
                gap: 1.5rem;
                margin: 0 1.5rem 2rem 1.5rem !important;
                padding: 2rem 1.5rem !important;
            }
            
            .grid-view {
                margin: 0 1.5rem !important;
            }
            
            .totals-footer {
                margin: 2rem 1.5rem !important;
            }
            
            .totals-inner {
                padding: 1.5rem !important;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <h1>| Detalle de Asiento Contable</h1>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/LibroMayor.aspx">&mdash; Volver al Historial</asp:HyperLink>
            </div>
            
            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <asp:Panel ID="pnlContenido" runat="server">
                <div class="asiento-header">
                    <div>
                        <strong>Asiento ID:</strong>
                        <asp:Label ID="lblAsientoID" runat="server" Text="---"></asp:Label>
                    </div>
                    <div>
                        <strong>Fecha del Asiento:</strong>
                        <asp:Label ID="lblFecha" runat="server" Text="---"></asp:Label>
                    </div>
                    <div class="desc">
                        <strong>Descripción:</strong>
                        <asp:Label ID="lblDescripcion" runat="server" Text="---"></asp:Label>
                    </div>
                </div>

                <asp:GridView ID="gvDetalle" runat="server"
                    CssClass="grid-view"
                    AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="NombreCuenta" HeaderText="Cuenta Contable" />
                        <asp:BoundField DataField="Debe" HeaderText="Debe" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Haber" HeaderText="Haber" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    </Columns>
                    <EmptyDataTemplate>
                        No se encontraron detalles para este asiento.
                    </EmptyDataTemplate>
                </asp:GridView>
                
                <div class="totals-footer">
                    <div class="totals-inner">
                        <div class="totals-row">
                            <span class="totals-label">Total Debe</span>
                            <asp:Label ID="lblTotalDebe" runat="server" Text="0.00" CssClass="totals-value"></asp:Label>
                        </div>
                        <div class="totals-row">
                            <span class="totals-label">Total Haber</span>
                            <asp:Label ID="lblTotalHaber" runat="server" Text="0.00" CssClass="totals-value"></asp:Label>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>