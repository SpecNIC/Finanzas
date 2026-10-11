-- ============================================================================
-- Script para modificar la tabla EstadosResultados según los nuevos requisitos
-- Mejoras implementadas:
-- 1. Quitar FechaInicio y FechaFin
-- 2. Agregar Periodo (VARCHAR)
-- 3. Modificar DetalleEstadoResultado para tener campos específicos
-- 4. Mejor manejo de errores y validaciones
-- 5. Transacciones para garantizar integridad
-- ============================================================================

USE software;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON; -- Rollback automático en caso de error

PRINT '--- INICIANDO MODIFICACIÓN DE TABLAS DE ESTADO DE RESULTADO ---';
PRINT 'Fecha de ejecución: ' + CONVERT(VARCHAR(23), GETDATE(), 121);

BEGIN TRY
    BEGIN TRANSACTION;

    -- =========================================
    -- PASO 1: Verificar existencia de tablas y eliminar datos existentes
    -- =========================================
    PRINT '';
    PRINT '--- PASO 1: Verificando y limpiando datos existentes ---';
    
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleEstadoResultado]') AND type in (N'U'))
    BEGIN
        PRINT 'Eliminando registros de DetalleEstadoResultado...';
        DELETE FROM DetalleEstadoResultado;
        PRINT '  ✓ Registros eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT '  ⚠ Tabla DetalleEstadoResultado no existe. Se creará más adelante.';
    END

    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND type in (N'U'))
    BEGIN
        PRINT 'Eliminando registros de EstadosResultados...';
        DELETE FROM EstadosResultados;
        PRINT '  ✓ Registros eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT '  ⚠ Tabla EstadosResultados no existe. Se creará más adelante.';
    END

    -- =========================================
    -- PASO 2: Modificar tabla EstadosResultados
    -- =========================================
    PRINT '';
    PRINT '--- PASO 2: Modificando tabla EstadosResultados ---';
    
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND type in (N'U'))
    BEGIN
        PRINT '  ⚠ Tabla EstadosResultados no existe. Creándola...';
        CREATE TABLE EstadosResultados (
            EstadoResultadoID INT PRIMARY KEY IDENTITY(1,1),
            Periodo VARCHAR(50) NOT NULL DEFAULT '2024',
            UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioID),
            FechaGeneracion DATETIME NOT NULL DEFAULT GETDATE()
        );
        PRINT '  ✓ Tabla EstadosResultados creada exitosamente.';
    END
    ELSE
    BEGIN
        -- Eliminar columnas FechaInicio y FechaFin si existen
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'FechaInicio')
        BEGIN
            PRINT 'Eliminando columna FechaInicio...';
            ALTER TABLE EstadosResultados DROP COLUMN FechaInicio;
            PRINT '  ✓ Columna FechaInicio eliminada.';
        END
        ELSE
        BEGIN
            PRINT '  ℹ Columna FechaInicio no existe.';
        END

        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'FechaFin')
        BEGIN
            PRINT 'Eliminando columna FechaFin...';
            ALTER TABLE EstadosResultados DROP COLUMN FechaFin;
            PRINT '  ✓ Columna FechaFin eliminada.';
        END
        ELSE
        BEGIN
            PRINT '  ℹ Columna FechaFin no existe.';
        END

        -- Agregar columna Periodo si no existe
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND name = 'Periodo')
        BEGIN
            PRINT 'Agregando columna Periodo...';
            ALTER TABLE EstadosResultados ADD Periodo VARCHAR(50) NOT NULL DEFAULT '2024';
            PRINT '  ✓ Columna Periodo agregada con valor por defecto ''2024''.';
        END
        ELSE
        BEGIN
            PRINT '  ℹ Columna Periodo ya existe.';
        END
    END

    -- =========================================
    -- PASO 3: Eliminar y recrear DetalleEstadoResultado con nueva estructura
    -- =========================================
    PRINT '';
    PRINT '--- PASO 3: Recreando tabla DetalleEstadoResultado ---';
    
    -- Eliminar tabla si existe
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleEstadoResultado]') AND type in (N'U'))
    BEGIN
        PRINT 'Eliminando tabla DetalleEstadoResultado existente...';
        DROP TABLE DetalleEstadoResultado;
        PRINT '  ✓ Tabla DetalleEstadoResultado eliminada.';
    END

    -- Crear nueva tabla con campos específicos
    PRINT 'Creando tabla DetalleEstadoResultado con nueva estructura...';
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
    PRINT '  ✓ Tabla DetalleEstadoResultado creada exitosamente.';
    PRINT '  ✓ Estructura: 6 campos de entrada + 6 campos calculados.';

    -- =========================================
    -- PASO 4: Verificar integridad de las tablas
    -- =========================================
    PRINT '';
    PRINT '--- PASO 4: Verificando integridad de las tablas ---';
    
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EstadosResultados]') AND type in (N'U'))
        AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleEstadoResultado]') AND type in (N'U'))
    BEGIN
        PRINT '  ✓ Ambas tablas existen correctamente.';
        PRINT '  ✓ Relación de clave foránea establecida.';
    END
    ELSE
    BEGIN
        RAISERROR('Error: No se pudieron crear todas las tablas necesarias.', 16, 1);
    END

    COMMIT TRANSACTION;
    PRINT '';
    PRINT '--- MODIFICACIÓN COMPLETADA EXITOSAMENTE ---';
    PRINT 'Fecha de finalización: ' + CONVERT(VARCHAR(23), GETDATE(), 121);

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '--- ERROR EN LA MODIFICACIÓN ---';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
    PRINT 'Error Procedure: ' + ISNULL(ERROR_PROCEDURE(), 'N/A');
    PRINT '';
    PRINT 'Se ha realizado un ROLLBACK de todos los cambios.';
    
    THROW;
END CATCH;

SET NOCOUNT OFF;
