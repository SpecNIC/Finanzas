<%-- 
  Página: RegistrarActivo.aspx
  Propósito: Formulario para registrar un nuevo Activo Fijo.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistrarActivo.aspx.cs" Inherits="Software.RegistrarActivo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrar Activo Fijo</title>
    <link href="Styles/Contenido.css?v=3.4" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap');
        
        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 2rem 2.5rem;
            margin: 1.5rem 0;
        }
        
        @media (max-width: 768px) {
            .form-grid {
                grid-template-columns: 1fr;
                gap: 1.5rem;
            }
        }
        
        .form-group {
            display: flex;
            flex-direction: column;
        }
        
        .form-group label {
            font-weight: 600;
            margin-bottom: 0.75rem;
            color: #334155;
            font-size: 0.9375rem;
            font-family: 'Inter', sans-serif;
            letter-spacing: -0.01em;
            line-height: 1.4;
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
        
        textarea.form-control {
            resize: vertical;
            min-height: 100px;
        }
        
        .full-width {
            grid-column: 1 / -1;
        }
        
        fieldset {
            border: 1px solid #e2e8f0 !important;
            border-radius: 16px !important;
            padding: 2.5rem 3rem !important;
            margin: 2.5rem 3rem 1.5rem 3rem !important;
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
        
        hr.full-width {
            border: 0;
            border-top: 2px solid #e2e8f0;
            margin: 2rem 0 1rem 0;
            grid-column: 1 / -1;
            position: relative;
        }
        
        hr.full-width::after {
            content: '';
            position: absolute;
            left: 0;
            top: -1px;
            width: 60px;
            height: 2px;
            background: linear-gradient(90deg, #2c5aa0 0%, #1a4d8c 100%);
            border-radius: 2px;
        }
        
        .action-bar {
            justify-content: flex-end;
            margin-top: 2rem;
            padding: 2rem 3rem !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="content-container">
            
            <div class="content-header">
                <h1>| Registrar Nuevo Activo Fijo</h1>
                <asp:HyperLink ID="linkVolver" runat="server" CssClass="back-link" NavigateUrl="~/LibroMayor.aspx">&mdash; Volver</asp:HyperLink>
            </div>
            
            <asp:Label ID="lblMensaje" runat="server" CssClass="feedback-message" Visible="false"></asp:Label>

            <fieldset>
                <legend>Detalles del Activo</legend>

                <div class="form-grid">

                    <div class="form-group">
                        <asp:Label ID="lblNombreActivo" runat="server" Text="Nombre del Activo" AssociatedControlID="txtNombreActivo"></asp:Label>
                        <asp:TextBox ID="txtNombreActivo" runat="server" CssClass="form-control" placeholder="Ej. Computadora Dell XPS 15"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblFechaAdquisicion" runat="server" Text="Fecha de Adquisición" AssociatedControlID="txtFechaAdquisicion"></asp:Label>
                        <asp:TextBox ID="txtFechaAdquisicion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="form-group full-width">
                        <asp:Label ID="lblDescripcion" runat="server" Text="Descripción (Opcional)" AssociatedControlID="txtDescripcion"></asp:Label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblCostoAdquisicion" runat="server" Text="Costo de Adquisición (C$)" AssociatedControlID="txtCostoAdquisicion"></asp:Label>
                        <asp:TextBox ID="txtCostoAdquisicion" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblValorRescate" runat="server" Text="Valor de Rescate (C$)" AssociatedControlID="txtValorRescate"></asp:Label>
                        <asp:TextBox ID="txtValorRescate" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblVidaUtil" runat="server" Text="Vida &Uacute;til (A&ntilde;os)" AssociatedControlID="txtVidaUtil"></asp:Label>
                        <asp:TextBox ID="txtVidaUtil" runat="server" CssClass="form-control" TextMode="Number" step="1" placeholder="Ej. 5"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        </div>
                    
                    <hr class="full-width" />

                    <div class="form-group full-width">
                        <asp:Label ID="lblCuentaActivo" runat="server" Text="Cuenta Contable del Activo" AssociatedControlID="ddlCuentaActivo"></asp:Label>
                        <asp:DropDownList ID="ddlCuentaActivo" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <asp:Label ID="lblCuentaGastoDepr" runat="server" Text="Cuenta de Gasto por Depreciación" AssociatedControlID="ddlCuentaGastoDepr"></asp:Label>
                        <asp:DropDownList ID="ddlCuentaGastoDepr" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblCuentaDeprAcum" runat="server" Text="Cuenta de Depreciación Acumulada" AssociatedControlID="ddlCuentaDeprAcum"></asp:Label>
                        <asp:DropDownList ID="ddlCuentaDeprAcum" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>

                </div>
            </fieldset>

            <div class="action-bar">
                 <asp:Button ID="btnGuardarActivo" runat="server" Text="Guardar Activo" 
                    CssClass="btn btn-success" OnClick="btnGuardarActivo_Click" />
            </div>

        </div>
    </form>
</body>
</html>