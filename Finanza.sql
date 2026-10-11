Create database software
use software
go
--==============================================================================
-- SECCION 1: TABLAS DE USUARIOS Y AUTENTICACION
--==============================================================================

CREATE TABLE Usuarios (
    UsuarioID INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario VARCHAR(100) NOT NULL UNIQUE,
    
    -- IMPORTANTE: Almacena SIEMPRE un HASH de la contraseña, NUNCA texto plano.
    -- Tu codigo en C# debe hashear la contraseña antes de guardarla aqui.
    PasswordHash VARCHAR(256) NOT NULL, 
    
    NombreCompleto VARCHAR(200) NOT NULL,
    CorreoElectronico VARCHAR(200) NULL,
    EsAdmin BIT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, EsAdmin)
VALUES (
    'admin', 
    'a3f01e1a53906660b949d9f58c75d658253e20e8301887e0766398b95573e83f', 
    'Administrador del Sistema', 
    1
);


--==============================================================================
-- SECCION 2: TABLAS CENTRALES DE CONTABILIDAD (LIBRO MAYOR)
--==============================================================================

-- Catalogo de Cuentas (El plan contable de la empresa)
CREATE TABLE CatalogoCuentas (
    CuentaID INT PRIMARY KEY IDENTITY(1,1),
    CodigoCuenta VARCHAR(20) NOT NULL UNIQUE, -- Ej: '101', '101.01', '401'
    NombreCuenta VARCHAR(200) NOT NULL,
    -- Tipos: Activo, Pasivo, Patrimonio, Ingreso, Gasto (o mas detallado)
    TipoCuenta VARCHAR(50) NOT NULL 
);

-- Encabezado de Asientos (Libro Diario)
CREATE TABLE AsientosDiario (
    AsientoID INT PRIMARY KEY IDENTITY(1,1),
    FechaAsiento DATE NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    
    -- El 'Autor' del asiento
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID), 
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);

-- Detalle de Asientos (El "Libro Mayor" en si)
CREATE TABLE DetalleAsientos (
    DetalleID INT PRIMARY KEY IDENTITY(1,1),
    -- Un asiento se compone de multiples detalles
    AsientoID INT NOT NULL FOREIGN KEY REFERENCES AsientosDiario(AsientoID) ON DELETE CASCADE, 
    -- Cada detalle afecta a una cuenta del catalogo
    CuentaID INT NOT NULL FOREIGN KEY REFERENCES CatalogoCuentas(CuentaID), 
    
    -- Principio de partida doble
    Debe DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    Haber DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    
    -- Restriccion para asegurar que no haya montos negativos
    CONSTRAINT CHK_MontosPositivos CHECK (Debe >= 0 AND Haber >= 0) 
);


--==============================================================================
-- SECCION 3: TABLAS DE DEPRECIACIONES Y AMORTIZACIONES
--==============================================================================

CREATE TABLE ActivosFijos (
    ActivoID INT PRIMARY KEY IDENTITY(1,1),
    NombreActivo VARCHAR(200) NOT NULL,
    Descripcion VARCHAR(500) NULL,
    FechaAdquisicion DATE NOT NULL,
    CostoAdquisicion DECIMAL(18, 2) NOT NULL,
    VidaUtilAnios INT NOT NULL,
    ValorRescate DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    -- Para identificar las cuentas contables asociadas (ej: 'Edificios', 'Gasto Depr.')
    CuentaActivoID INT NULL FOREIGN KEY REFERENCES CatalogoCuentas(CuentaID),
    CuentaGastoDeprID INT NULL FOREIGN KEY REFERENCES CatalogoCuentas(CuentaID),
    CuentaDeprAcumID INT NULL FOREIGN KEY REFERENCES CatalogoCuentas(CuentaID)
);

-- Almacena cada calculo de depreciacion individual (mensual o anual)
CREATE TABLE RegistrosDepreciacion (
    DepreciacionID INT PRIMARY KEY IDENTITY(1,1),
    ActivoID INT NOT NULL FOREIGN KEY REFERENCES ActivosFijos(ActivoID),
    FechaDepreciacion DATE NOT NULL,
    MontoDepreciado DECIMAL(18, 2) NOT NULL,
    -- (Opcional) Enlaza este registro al asiento contable que lo registro
    AsientoID INT NULL FOREIGN KEY REFERENCES AsientosDiario(AsientoID) 
);


--==============================================================================
-- SECCION 4: TABLAS DE "SNAPSHOTS" DE REPORTES FINANCIEROS
-- (Para cumplir con el requisito de "mostrar en una tabla")
--==============================================================================

-- ----- 4.1 Balance General -----
CREATE TABLE BalancesGenerales (
    BalanceID INT PRIMARY KEY IDENTITY(1,1),
    -- El Balance es una "foto" a una fecha especifica
    FechaReporte DATE NOT NULL, 
    
    -- El "Autor" que genera el reporte
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID), 
    FechaGeneracion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE DetalleBalance (
    DetalleBalanceID INT PRIMARY KEY IDENTITY(1,1),
    BalanceID INT NOT NULL FOREIGN KEY REFERENCES BalancesGenerales(BalanceID) ON DELETE CASCADE,
    CategoriaCuenta VARCHAR(100) NOT NULL, -- Ej: 'Activo Corriente', 'Pasivo No Corriente'
    NombreCuenta VARCHAR(200) NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL
);

-- ----- 4.2 Estado de Resultado -----
CREATE TABLE EstadosResultados (
    EstadoResultadoID INT PRIMARY KEY IDENTITY(1,1),
    Periodo VARCHAR(50) NOT NULL, -- Ej: '2024', 'Enero 2024', 'Q1 2024'
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    FechaGeneracion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE DetalleEstadoResultado (
    DetalleERID INT PRIMARY KEY IDENTITY(1,1),
    EstadoResultadoID INT NOT NULL FOREIGN KEY REFERENCES EstadosResultados(EstadoResultadoID) ON DELETE CASCADE,
    -- Campos que se piden al usuario
    IngresosVentas DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CostoBienesVendidos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GastosOperativos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GastosIntereses DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TasaImpuestos DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Porcentaje (ej: 40.00 para 40%)
    DividendosEfectivo DECIMAL(18, 2) NOT NULL DEFAULT 0,
    -- Campos calculados (guardados para referencia)
    UtilidadBruta DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadOperativa DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadNetaAntesImpuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Impuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadNetaDespuesImpuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GananciasRetenidas DECIMAL(18, 2) NOT NULL DEFAULT 0
);

-- ----- 4.3 Estado de Origen y Aplicacion -----
CREATE TABLE EstadosFlujo (
    EstadoFlujoID INT PRIMARY KEY IDENTITY(1,1),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    FechaGeneracion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE DetalleEstadoFlujo (
    DetalleFlujoID INT PRIMARY KEY IDENTITY(1,1),
    EstadoFlujoID INT NOT NULL FOREIGN KEY REFERENCES EstadosFlujo(EstadoFlujoID) ON DELETE CASCADE,
    -- Ej: 'Actividades de Operaci�n', 'Actividades de Inversion', 'Act. de Financiamiento'
    CategoriaFlujo VARCHAR(100) NOT NULL, 
    NombreLinea VARCHAR(200) NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL
);

UPDATE Usuarios
SET 
    NombreUsuario = LTRIM(RTRIM(NombreUsuario)),
    PasswordHash = LTRIM(RTRIM(PasswordHash)),
    NombreCompleto = LTRIM(RTRIM(NombreCompleto))
WHERE 
    UsuarioID = 1;



USE software;
GO

PRINT '--- INICIANDO REINICIO TOTAL DE DATOS ---';

-- =========================================
-- FASE 1: LIMPIEZA DE TABLAS (HIJOS PRIMERO)
-- =========================================
PRINT 'Limpiando tablas existentes...';
DELETE FROM DetalleAsientos;
DELETE FROM RegistrosDepreciacion;
DELETE FROM DetalleBalance;
DELETE FROM DetalleEstadoResultado;
DELETE FROM DetalleEstadoFlujo;

-- (Ahora podemos borrar los padres de los detalles)
DELETE FROM AsientosDiario;
DELETE FROM ActivosFijos; 
DELETE FROM BalancesGenerales;
DELETE FROM EstadosResultados;
DELETE FROM EstadosFlujo;

-- (Ahora podemos borrar el catalogo)
DELETE FROM CatalogoCuentas;

-- (CORRECCION AQUI: Borra TODOS los usuarios, no solo los no-admin)
PRINT 'Limpiando todos los usuarios...';
DELETE FROM Usuarios;
GO

-- =========================================
-- FASE 2: REINICIO DE CONTADORES (IDENTITY)
-- =========================================
PRINT 'Reiniciando contadores de ID...';
DBCC CHECKIDENT ('DetalleAsientos', RESEED, 0);
-- ... (resto de DBCC CHECKIDENTs) ...
DBCC CHECKIDENT ('CatalogoCuentas', RESEED, 0);
DBCC CHECKIDENT ('Usuarios', RESEED, 0); -- Reiniciamos a 0 para que el primer INSERT sea 1
GO

-- =========================================
-- FASE 2.5: INSERTAR USUARIO ADMIN
-- =========================================
PRINT 'Insertando usuario admin por defecto...';
INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, EsAdmin)
VALUES (
    'admin', 
    'a3f01e1a53906660b949d9f58c75d658253e20e8301887e0766398b95573e83f', -- Hash para "231205"
    'Administrador del Sistema', 
    1
);
GO

-- =========================================
-- FASE 3: INSERTAR CAT�LOGO DE CUENTAS
-- =========================================
PRINT 'Insertando Catalogo de Cuentas Base...';

-- 1. ACTIVOS (IDs 1-5)
INSERT INTO CatalogoCuentas (CodigoCuenta, NombreCuenta, TipoCuenta)
VALUES 
('101', 'Efectivo y Bancos', 'Activo'),
('102', 'Cuentas por Cobrar', 'Activo'),
('103', 'Inventario', 'Activo'),
('104', 'Edificios', 'Activo'),
('105', 'Depreciacion Acumulada de Edificios', 'Activo');

-- 2. PASIVOS (IDs 6-7)
INSERT INTO CatalogoCuentas (CodigoCuenta, NombreCuenta, TipoCuenta)
VALUES 
('201', 'Cuentas por Pagar', 'Pasivo'),
('202', 'Pr�stamos Bancarios', 'Pasivo');

-- 3. PATRIMONIO (ID 8)
INSERT INTO CatalogoCuentas (CodigoCuenta, NombreCuenta, TipoCuenta)
VALUES 
('301', 'Capital Social', 'Patrimonio');

-- 4. INGRESOS (ID 9)
INSERT INTO CatalogoCuentas (CodigoCuenta, NombreCuenta, TipoCuenta)
VALUES 
('401', 'Ingresos por Servicios', 'Ingreso');

-- 5. GASTOS (IDs 10-13)
INSERT INTO CatalogoCuentas (CodigoCuenta, NombreCuenta, TipoCuenta)
VALUES 
('501', 'Gasto por Salarios', 'Gasto'),
('502', 'Gasto por Alquiler', 'Gasto'),
('503', 'Gasto por Servicios Basicos', 'Gasto'),
('504', 'Gasto por Depreciacion', 'Gasto');
GO

PRINT 'Catalogo de Cuentas insertado!';

-- =========================================
-- FASE 4: INSERTAR DATOS CONTABLES (ASIENTOS)
-- =========================================
PRINT 'Insertando datos de prueba (Asientos)...';

DECLARE @UsuarioID INT = 1; -- Asume que tu usuario admin es el ID 1
DECLARE @AsientoID INT;

-- DATOS A�O 2024
PRINT 'Insertando datos 2024...';
-- 1. Asiento de Apertura 2024
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2024-01-01', 'Asiento de Apertura 2024', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY(); 
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 1, 50000.00, 0.00), -- ID 1: Efectivo y Bancos
(@AsientoID, 8, 0.00, 50000.00); -- ID 8: Capital Social

-- 2. Ingresos del A�o 2024
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2024-12-31', 'Ingresos Totales 2024', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 1, 80000.00, 0.00), -- ID 1: Efectivo y Bancos
(@AsientoID, 9, 0.00, 80000.00); -- ID 9: Ingresos por Servicios

-- 3. Gastos del A�o 2024
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2024-12-31', 'Gastos Totales 2024', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 10, 30000.00, 0.00), -- ID 10: Gasto por Salarios
(@AsientoID, 11, 10000.00, 0.00), -- ID 11: Gasto por Alquiler
(@AsientoID, 1, 0.00, 40000.00); -- ID 1: Efectivo y Bancos

-- DATOS A�O 2025
PRINT 'Insertando datos 2025...';
-- 4. Pr�stamo Bancario 2025
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2025-03-15', 'Pr�stamo Bancario Hipotecario', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 1, 20000.00, 0.00), -- ID 1: Efectivo y Bancos
(@AsientoID, 7, 0.00, 20000.00); -- ID 7: Pr�stamos Bancarios

-- 5. Compra de Edificio 2025
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2025-06-01', 'Compra de Edificio de Oficinas', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 4, 30000.00, 0.00), -- ID 4: Edificios
(@AsientoID, 1, 0.00, 30000.00); -- ID 1: Efectivo y Bancos

-- 6. Ingresos del A�o 2025
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2025-12-31', 'Ingresos Totales 2025', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 1, 120000.00, 0.00), -- ID 1: Efectivo y Bancos
(@AsientoID, 9, 0.00, 120000.00); -- ID 9: Ingresos por Servicios

-- 7. Gastos del A�o 2025
INSERT INTO AsientosDiario (FechaAsiento, Descripcion, UsuarioID)
VALUES ('2025-12-31', 'Gastos Totales 2025', @UsuarioID);
SET @AsientoID = SCOPE_IDENTITY();
INSERT INTO DetalleAsientos (AsientoID, CuentaID, Debe, Haber)
VALUES 
(@AsientoID, 10, 40000.00, 0.00), -- ID 10: Gasto por Salarios
(@AsientoID, 11, 15000.00, 0.00), -- ID 11: Gasto por Alquiler
(@AsientoID, 1, 0.00, 55000.00); -- ID 1: Efectivo y Bancos
GO

PRINT '--- REINICIO COMPLETADO ---';
PRINT 'Datos de prueba insertados correctamente.';


USE software;
GO

PRINT 'Actualizando el hash del usuario admin...';

UPDATE Usuarios
SET
    PasswordHash = 'c2049a1a5f49756ae3f74f98f47d01b61b565494e08e6d35468a62272437ca6d'
WHERE 
    NombreUsuario = 'admin';

PRINT 'Hash del admin corregido! Ahora puedes iniciar sesion con admin';
GO

-- Script para modificar la tabla EstadosResultados según los nuevos requisitos
-- 1. Quitar FechaInicio y FechaFin
-- 2. Agregar Periodo (VARCHAR)
-- 3. Modificar DetalleEstadoResultado para tener campos específicos

USE software;
GO

PRINT '--- INICIANDO MODIFICACIÓN DE TABLAS DE ESTADO DE RESULTADO ---';

-- =========================================
-- PASO 1: Eliminar datos existentes si es necesario
-- =========================================
PRINT 'Eliminando datos existentes...';
DELETE FROM DetalleEstadoResultado;
DELETE FROM EstadosResultados;

-- =========================================
-- PASO 2: Modificar tabla EstadosResultados
-- =========================================
PRINT 'Modificando tabla EstadosResultados...';

-- Eliminar columnas FechaInicio y FechaFin si existen
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'FechaInicio')
BEGIN
    ALTER TABLE EstadosResultados DROP COLUMN FechaInicio;
    PRINT 'Columna FechaInicio eliminada.';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'FechaFin')
BEGIN
    ALTER TABLE EstadosResultados DROP COLUMN FechaFin;
    PRINT 'Columna FechaFin eliminada.';
END

-- Agregar columna Periodo si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'Periodo')
BEGIN
    ALTER TABLE EstadosResultados ADD Periodo VARCHAR(50) NOT NULL DEFAULT '2024';
    PRINT 'Columna Periodo agregada.';
END

-- =========================================
-- PASO 3: Eliminar y recrear DetalleEstadoResultado con nueva estructura
-- =========================================
PRINT 'Recreando tabla DetalleEstadoResultado...';

-- Eliminar tabla si existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleEstadoResultado]') AND type in (N'U'))
BEGIN
    DROP TABLE DetalleEstadoResultado;
    PRINT 'Tabla DetalleEstadoResultado eliminada.';
END

-- Crear nueva tabla con campos específicos
CREATE TABLE DetalleEstadoResultado (
    DetalleERID INT PRIMARY KEY IDENTITY(1,1),
    EstadoResultadoID INT NOT NULL FOREIGN KEY REFERENCES EstadosResultados(EstadoResultadoID) ON DELETE CASCADE,
    -- Campos que se piden al usuario
    IngresosVentas DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CostoBienesVendidos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GastosOperativos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GastosIntereses DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TasaImpuestos DECIMAL(5, 2) NOT NULL DEFAULT 0, -- Porcentaje (ej: 40.00 para 40%)
    DividendosEfectivo DECIMAL(18, 2) NOT NULL DEFAULT 0,
    -- Campos calculados (guardados para referencia)
    UtilidadBruta DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadOperativa DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadNetaAntesImpuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Impuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    UtilidadNetaDespuesImpuestos DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GananciasRetenidas DECIMAL(18, 2) NOT NULL DEFAULT 0
);

PRINT 'Tabla DetalleEstadoResultado recreada con nueva estructura.';
PRINT '--- MODIFICACIÓN COMPLETADA ---';

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

