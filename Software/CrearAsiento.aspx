<%-- 
  Página: CrearAsiento.aspx
  Propósito: Formulario para crear un nuevo asiento contable (Debe/Haber).
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearAsiento.aspx.cs" Inherits="Software.CrearAsiento" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Crear Asiento Contable</title>
    <link href="Styles/Contenido.css?v=3.4" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');
        
        .grid-view {
            margin: 0 3rem 2rem 3rem !important;
        }
        
        .grid-view td { 
            padding: 14px 20px !important; 
            font-family: 'Inter', sans-serif;
        }
        
        .line-entry { 
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%); 
            border: 1px solid #e2e8f0; 
            border-radius: 16px; 
            padding: 2.5rem 3rem; 
            display: flex; 
            gap: 1.5rem; 
            align-items: flex-end; 
            margin: 0 3rem 2.5rem 3rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
            position: relative;
            overflow: hidden;
        }
        
        .line-entry::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        .line-entry > div { 
            flex: 1; 
        }
        
        .line-entry > div:last-child {
            flex: 0 0 auto;
            min-width: 160px;
        }
        
        .line-entry label { 
            font-weight: 600; 
            display: block; 
            margin-bottom: 0.75rem;
            color: #334155;
            font-size: 0.9375rem;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            line-height: 1.4;
        }
        
        @media (max-width: 768px) {
            .line-entry {
                flex-direction: column;
                align-items: stretch;
                gap: 1.5rem;
                padding: 2rem 1.5rem;
                margin: 0 1.5rem 2rem 1.5rem;
            }
            
            .line-entry > div:last-child {
                min-width: 100%;
            }
        }
        
        .form-control { 
            width: 100%; 
            padding: 14px 20px; 
            font-size: 0.9375rem; 
            border: 2px solid #e2e8f0; 
            border-radius: 10px; 
            box-sizing: border-box;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            background-color: #ffffff;
            font-family: 'Inter', sans-serif;
            font-weight: 400;
            letter-spacing: -0.01em;
            line-height: 1.5;
            color: #1a1a1a;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
        }
        
        .form-control:hover {
            border-color: #cbd5e0;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.06);
        }
        
        .form-control:focus {
            border-color: #2c5aa0;
            box-shadow: 0 0 0 4px rgba(44, 90, 160, 0.12), 0 2px 8px rgba(0, 0, 0, 0.08);
            outline: none;
            background-color: #ffffff;
            font-weight: 500;
        }
        
        .form-control::placeholder {
            color: #94a3b8;
            font-weight: 400;
        }
        
        .totals { 
            text-align: right; 
            font-size: 1.125rem; 
            font-weight: 600; 
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
        
        .totals-row:last-child {
            border-bottom: none;
            padding-bottom: 0;
            margin-top: 1rem;
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
            color: #1a4d8c;
        }
        
        .totals-ok {
            color: #059669 !important;
        }
        
        .totals-error { 
            color: #dc2626 !important;
            background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%);
            padding: 1rem 1.5rem;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.75rem;
            margin-top: 1.5rem;
            border: 2px solid #fecaca;
            font-weight: 600;
            font-size: 0.9375rem;
            color: #991b1b;
            box-shadow: 0 2px 8px rgba(220, 38, 38, 0.1);
        }
        
        .totals-error::before {
            content: '⚠';
            font-size: 1.25rem;
        }
        
        .totals-balanced {
            background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
            border: 2px solid #86efac;
        }
        
        .totals-balanced .totals-value {
            color: #059669 !important;
        }
        
        fieldset {
            border: 1px solid #e2e8f0 !important;
            border-radius: 16px !important;
            padding: 2.5rem 3rem !important;
            margin: 0 3rem 2.5rem 3rem !important;
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
            position: relative;
            overflow: hidden;
        }
        
        fieldset::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
        }
        
        legend {
            font-size: 1.375rem !important;
            font-weight: 700 !important;
            color: #1a4d8c !important;
            padding: 0 1.25rem !important;
            letter-spacing: -0.4px;
            font-family: 'Inter', sans-serif;
            position: relative;
        }
        
        legend::before {
            content: '';
            position: absolute;
            left: -0.75rem;
            top: 50%;
            transform: translateY(-50%);
            width: 4px;
            height: 60%;
            background: linear-gradient(135deg, #2c5aa0 0%, #1a4d8c 100%);
            border-radius: 2px;
        }
        
        h4 {
            color: #1a4d8c;
            font-size: 1.5rem;
            font-weight: 700;
            margin: 3rem 3rem 1.75rem 3rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid #e2e8f0;
            letter-spacing: -0.4px;
            line-height: 1.3;
            font-family: 'Inter', sans-serif;
            position: relative;
            display: inline-block;
            padding-right: 2rem;
        }
        
        h4::after {
            content: '';
            position: absolute;
            bottom: -2px;
            left: 0;
            width: 60px;
            height: 2px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
            border-radius: 2px;
        }
        
        .form-row {
            display: flex;
            gap: 2rem;
        }
        
        @media (max-width: 768px) {
            .form-row {
                flex-direction: column;
                gap: 1.5rem;
            }
            
            fieldset, h4, .line-entry, .totals {
                margin-left: 1.5rem !important;
                margin-right: 1.5rem !important;
                padding: 2rem 1.5rem !important;
            }
        }
        
        .form-col-1 {
            flex: 1;
        }
        
        .form-col-3 {
            flex: 3;
        }
        
        .action-bar {
            padding: 2rem 3rem !important;
            margin-top: 1rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <div class="header-content">
                    <h1>Crear Nuevo Asiento Contable</h1>
                    <p class="header-subtitle">Registra movimientos contables con el sistema de partida doble (Debe/Haber)</p>
                </div>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/LibroMayor.aspx">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M19 12H5M12 19l-7-7 7-7"/>
                    </svg>
                    Volver
                </asp:HyperLink>
            </div>
            
            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <fieldset>
                <legend>Datos del Asiento</legend>
                <div class="form-row">
                    <div class="form-col-1">
                        <label for="<%=txtFechaAsiento.ClientID%>">Fecha del Asiento</label>
                        <asp:TextBox ID="txtFechaAsiento" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-col-3">
                        <label for="<%=txtDescripcion.ClientID%>">Descripción</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" placeholder="Ej. Asiento de apertura, pago de nómina, etc."></asp:TextBox>
                    </div>
                </div>
            </fieldset>

            <h4>Agregar Líneas (Debe y Haber)</h4>
            <div class="line-entry">
                <div>
                    <label for="<%=ddlCuentas.ClientID%>">Cuenta Contable</label>
                    <asp:DropDownList ID="ddlCuentas" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div>
                    <label for="<%=txtDebe.ClientID%>">Debe</label>
                    <asp:TextBox ID="txtDebe" runat="server" CssClass="form-control" TextMode="Number" step="0.01">0.00</asp:TextBox>
                </div>
                <div>
                    <label for="<%=txtHaber.ClientID%>">Haber</label>
                    <asp:TextBox ID="txtHaber" runat="server" CssClass="form-control" TextMode="Number" step="0.01">0.00</asp:TextBox>
                </div>
                <div>
                    <asp:Button ID="btnAgregarLinea" runat="server" Text="Agregar Línea" CssClass="btn btn-primary" OnClick="btnAgregarLinea_Click" />
                </div>
            </div>

            <h4>Líneas del Asiento</h4>
            <asp:GridView ID="gvDetalleAsiento" runat="server"
                CssClass="grid-view"
                AutoGenerateColumns="False"
                DataKeyNames="IDTemporal"
                OnRowDeleting="gvDetalleAsiento_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="NombreCuenta" HeaderText="Cuenta" />
                    <asp:BoundField DataField="Debe" HeaderText="Debe" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Haber" HeaderText="Haber" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <asp:CommandField ShowDeleteButton="True" DeleteText="Eliminar" ControlStyle-CssClass="grid-select-link" />
                </Columns>
                <EmptyDataTemplate>
                    Aún no se han agregado líneas a este asiento.
                </EmptyDataTemplate>
            </asp:GridView>

            <div class="totals">
                <div class="totals-inner" id="totalsContainer" runat="server">
                    <div class="totals-row">
                        <span class="totals-label">Total Debe</span>
                        <asp:Label ID="lblTotalDebe" runat="server" Text="0.00" CssClass="totals-value"></asp:Label>
                    </div>
                    <div class="totals-row">
                        <span class="totals-label">Total Haber</span>
                        <asp:Label ID="lblTotalHaber" runat="server" Text="0.00" CssClass="totals-value"></asp:Label>
                    </div>
                    <asp:Label ID="lblDescuadre" runat="server" Text="¡Asiento descuadrado!" CssClass="totals-error" Visible="false"></asp:Label>
                </div>
            </div>

            <div class="action-bar">
                 <asp:Button ID="btnGuardarAsiento" runat="server" Text="Guardar Asiento Completo" 
                    CssClass="btn btn-success" OnClick="btnGuardarAsiento_Click" 
                    Enabled="false" />
            </div>

        </div>
    </form>
</body>
</html>