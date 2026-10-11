<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Software.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iniciar Sesión - Gestión Financiera</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Styles/Login.css?v=2.3" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="login-container">
            
            <div class="login-header">
                <div class="logo-container">
                    <div class="logo-wrapper">
                        <svg class="logo-svg" viewBox="0 0 80 80" xmlns="http://www.w3.org/2000/svg">
                            <defs>
                                <linearGradient id="logoGradient" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" style="stop-color:#1e3a5f;stop-opacity:1" />
                                    <stop offset="50%" style="stop-color:#2c5aa0;stop-opacity:1" />
                                    <stop offset="100%" style="stop-color:#4a90e2;stop-opacity:1" />
                                </linearGradient>
                                <filter id="shadow">
                                    <feDropShadow dx="0" dy="2" stdDeviation="3" flood-opacity="0.3"/>
                                </filter>
                            </defs>
                            <!-- Círculo de fondo con gradiente -->
                            <circle cx="40" cy="40" r="36" fill="url(#logoGradient)" filter="url(#shadow)"/>
                            
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
                <h2 class="app-title">Gestión Financiera</h2>
                <p class="app-subtitle">Sistema de Contabilidad Integrado</p>
            </div>
            
            <p class="subtitle">Bienvenido. Por favor, inicia sesión.</p>

            <div class="input-group">
                <asp:Label ID="lblUsuario" runat="server" AssociatedControlID="txtUsuario" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                        <circle cx="12" cy="7" r="4"></circle>
                    </svg>
                    <span class="label-text">Usuario</span>
                </asp:Label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Ingresa tu nombre de usuario" autocomplete="off" Text=""></asp:TextBox>
            </div>

            <div class="input-group">
                <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="input-label">
                    <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                        <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                    </svg>
                    <span class="label-text">Contraseña</span>
                </asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Ingresa tu contraseña" autocomplete="off" Text=""></asp:TextBox>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>

            <div class="button-group">
                <asp:Button ID="btnLogin" runat="server" Text="INGRESAR" CssClass="btn-login" OnClick="btnLogin_Click" />
            </div>

            <div class="register-link">
                <p>¿No tienes una cuenta? <a href="Registro.aspx">Regístrate aquí</a></p>
                </div>

        </div>

    </form>
</body>
</html>
