-- ============================================================================
-- Script de Migración de Seguridad
-- Actualiza la estructura de la base de datos para soportar hashes PBKDF2
-- ============================================================================

USE software;
GO

-- ============================================================================
-- 1. ACTUALIZAR TAMAÑO DEL CAMPO PasswordHash
-- PBKDF2 genera hashes más largos (salt:hash en base64)
-- ============================================================================
PRINT 'Actualizando tamaño del campo PasswordHash...';

-- Verificar si el campo necesita actualización
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Usuarios' 
    AND COLUMN_NAME = 'PasswordHash' 
    AND CHARACTER_MAXIMUM_LENGTH < 512
)
BEGIN
    ALTER TABLE Usuarios
    ALTER COLUMN PasswordHash VARCHAR(512) NOT NULL;
    
    PRINT 'Campo PasswordHash actualizado a VARCHAR(512).';
END
ELSE
BEGIN
    PRINT 'Campo PasswordHash ya tiene el tamaño correcto.';
END
GO

-- ============================================================================
-- 2. NOTA IMPORTANTE SOBRE MIGRACIÓN DE HASHES
-- ============================================================================
PRINT '';
PRINT '============================================================================';
PRINT 'IMPORTANTE: Migración de Hashes Existentes';
PRINT '============================================================================';
PRINT 'Los hashes SHA256 existentes NO se pueden convertir automáticamente a PBKDF2.';
PRINT 'El sistema soporta ambos formatos:';
PRINT '  - SHA256 (legacy): Hash hexadecimal de 64 caracteres';
PRINT '  - PBKDF2 (nuevo): Formato "salt:hash" en base64';
PRINT '';
PRINT 'Los usuarios con hash SHA256 podrán iniciar sesión normalmente.';
PRINT 'Sus contraseñas se actualizarán automáticamente a PBKDF2 en el próximo login.';
PRINT '============================================================================';
GO

-- ============================================================================
-- 3. CREAR ÍNDICE PARA MEJORAR RENDIMIENTO DE BÚSQUEDAS
-- ============================================================================
PRINT '';
PRINT 'Verificando índices en tabla Usuarios...';

IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_Usuarios_NombreUsuario' 
    AND object_id = OBJECT_ID('Usuarios')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Usuarios_NombreUsuario
    ON Usuarios(NombreUsuario);
    PRINT 'Índice IX_Usuarios_NombreUsuario creado.';
END
ELSE
BEGIN
    PRINT 'Índice IX_Usuarios_NombreUsuario ya existe.';
END
GO

-- ============================================================================
-- 4. AGREGAR COLUMNA PARA AUDITORÍA DE SEGURIDAD (OPCIONAL)
-- ============================================================================
PRINT '';
PRINT 'Verificando columna de última fecha de login...';

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Usuarios' 
    AND COLUMN_NAME = 'UltimoLogin'
)
BEGIN
    ALTER TABLE Usuarios
    ADD UltimoLogin DATETIME NULL;
    
    PRINT 'Columna UltimoLogin agregada para auditoría.';
END
ELSE
BEGIN
    PRINT 'Columna UltimoLogin ya existe.';
END
GO

-- ============================================================================
-- 5. AGREGAR COLUMNA PARA INTENTOS FALLIDOS (OPCIONAL)
-- ============================================================================
PRINT '';
PRINT 'Verificando columna de intentos fallidos...';

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Usuarios' 
    AND COLUMN_NAME = 'IntentosFallidos'
)
BEGIN
    ALTER TABLE Usuarios
    ADD IntentosFallidos INT NOT NULL DEFAULT 0,
        UltimoIntentoFallido DATETIME NULL;
    
    PRINT 'Columnas de intentos fallidos agregadas.';
END
ELSE
BEGIN
    PRINT 'Columnas de intentos fallidos ya existen.';
END
GO

-- ============================================================================
-- 6. VERIFICAR Y ACTUALIZAR CONFIGURACIÓN DE CIFRADO
-- ============================================================================
PRINT '';
PRINT '============================================================================';
PRINT 'VERIFICACIÓN DE CIFRADO';
PRINT '============================================================================';
PRINT 'Asegúrate de que en Web.config la cadena de conexión tenga:';
PRINT '  Encrypt=True';
PRINT '  TrustServerCertificate=True (solo para desarrollo)';
PRINT '';
PRINT 'Para producción, configura un certificado SSL válido y usa:';
PRINT '  Encrypt=True';
PRINT '  TrustServerCertificate=False';
PRINT '============================================================================';
GO

-- ============================================================================
-- 7. RESUMEN FINAL
-- ============================================================================
PRINT '';
PRINT '============================================================================';
PRINT 'MIGRACIÓN COMPLETADA';
PRINT '============================================================================';
PRINT 'La base de datos está lista para usar el nuevo sistema de seguridad:';
PRINT '  ✓ Campo PasswordHash actualizado';
PRINT '  ✓ Soporte para hashes PBKDF2 y SHA256 legacy';
PRINT '  ✓ Índices optimizados';
PRINT '  ✓ Columnas de auditoría agregadas';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '  1. Verificar que Web.config tenga Encrypt=True';
PRINT '  2. Los usuarios existentes seguirán funcionando';
PRINT '  3. Los nuevos registros usarán PBKDF2 automáticamente';
PRINT '  4. Los hashes antiguos se actualizarán en el próximo login';
PRINT '============================================================================';
GO



