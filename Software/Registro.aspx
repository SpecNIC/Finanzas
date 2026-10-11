<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="Software.Registro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro - Gestión Financiera</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Styles/Registro.css?v=2.3" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="register-container">
            
            <div class="register-header">
                <div class="logo-container">
                    <div class="logo-wrapper">
                        <svg class="logo-svg" viewBox="0 0 80 80" xmlns="http://www.w3.org/2000/svg">
                            <defs>
                                <linearGradient id="logoGradientReg" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" style="stop-color:#1e3a5f;stop-opacity:1" />
                                    <stop offset="50%" style="stop-color:#2c5aa0;stop-opacity:1" />
                                    <stop offset="100%" style="stop-color:#4a90e2;stop-opacity:1" />
                                </linearGradient>
                                <filter id="shadowReg">
                                    <feDropShadow dx="0" dy="2" stdDeviation="3" flood-opacity="0.3"/>
                                </filter>
                            </defs>
                            <!-- Círculo de fondo con gradiente -->
                            <circle cx="40" cy="40" r="36" fill="url(#logoGradientReg)" filter="url(#shadowReg)"/>
                            
                            <!-- Símbolo de gráfico financiero -->
                            <path d="M25 45 L30 35 L35 40 L40 25 L45 30 L50 20 L55 28" 
                                  stroke="white" 
                                  stroke-width="3.5" 
                                  fill="none" 
                                  stroke-linecap="round" 
                                  stroke-linejoin="round"
                                  opacity="0.95"/>
                            
                            <!-- Puntos en los vértices -->
                            <circle cx="25" cy="45" r="3" fill="white" opacity="0.9"/>
                            <circle cx="55" cy="28" r="3" fill="white" opacity="0.9"/>
                            
                            <!-- Icono de moneda estilizado -->
                            <circle cx="40" cy="50" r="8" fill="rgba(255,255,255,0.25)" stroke="white" stroke-width="2"/>
                            <text x="40" y="54" font-family="Arial, sans-serif" font-size="10" font-weight="bold" fill="white" text-anchor="middle">$</text>
                        </svg>
                    </div>
                </div>
                <h2 class="app-title">Crear una Cuenta</h2>
                <p class="app-subtitle">Sistema de Contabilidad Integrado</p>
            </div>
            
            <p class="subtitle">Ingresa tus datos para registrarte.</p>

            <asp:Label ID="lblMensaje" runat="server" CssClass="message-label" Visible="false"></asp:Label>

            <div class="input-group">
                <asp:Label ID="lblNombreCompleto" runat="server" AssociatedControlID="txtNombreCompleto" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                        <circle cx="12" cy="7" r="4"></circle>
                    </svg>
                    <span class="label-text">Nombre Completo</span>
                </asp:Label>
                <asp:TextBox ID="txtNombreCompleto" runat="server" CssClass="form-control" placeholder="Ej. Juan Pérez" autocomplete="off" Text=""></asp:TextBox>
            </div>

            <div class="input-group">
                <asp:Label ID="lblCorreo" runat="server" AssociatedControlID="txtCorreo" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                        <polyline points="22,6 12,13 2,6"></polyline>
                    </svg>
                    <span class="label-text">Correo Electrónico <span class="label-optional">(Opcional)</span></span>
                </asp:Label>
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="Ej. juan@correo.com" TextMode="Email" autocomplete="off" Text=""></asp:TextBox>
            </div>

            <div class="input-group">
                <asp:Label ID="lblUsuario" runat="server" AssociatedControlID="txtUsuario" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
                        <circle cx="8.5" cy="7" r="4"></circle>
                        <line x1="20" y1="8" x2="20" y2="14"></line>
                        <line x1="23" y1="11" x2="17" y2="11"></line>
                    </svg>
                    <span class="label-text">Nombre de Usuario</span>
                </asp:Label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Ej. jperez" autocomplete="off" Text=""></asp:TextBox>
            </div>

            <div class="input-group">
                <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                        <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                    </svg>
                    <span class="label-text">Contraseña</span>
                </asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Mínimo 6 caracteres" autocomplete="off" Text=""></asp:TextBox>
            </div>
            
            <div class="input-group">
                <asp:Label ID="lblConfirmarPassword" runat="server" AssociatedControlID="txtConfirmarPassword" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="20 6 9 17 4 12"></polyline>
                    </svg>
                    <span class="label-text">Confirmar Contraseña</span>
                </asp:Label>
                <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Repite tu contraseña" autocomplete="off" Text=""></asp:TextBox>
            </div>


            <div class="button-group">
                <asp:Button ID="btnRegistrar" runat="server" Text="REGISTRAR CUENTA" CssClass="btn-register" OnClick="btnRegistrar_Click" />
            </div>

            <div class="login-link">
                <p>¿Ya tienes una cuenta? <a href="Login.aspx">Inicia sesión aquí</a></p>
            </div>

        </div>

    </form>
</body>
</html>