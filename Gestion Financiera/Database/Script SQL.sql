-- =================================================================================
-- CREACIÓN DE BASE DE DATOS Y ESTRUCTURA DDL PARA LA APP DE GESTION DE FINANZAS
-- =================================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'FinananceDB')
BEGIN
    CREATE DATABASE FinananceDB;
END;
GO

USE FinananceDB;
GO

-- =================================================================================
-- TABLA: Usuarios
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        UserStatus BIT NOT NULL  DEFAULT 1,
        CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    )
END;
GO

-- =================================================================================
-- TABLA: Instituciones Bancarias
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BankInfo')
BEGIN
    CREATE TABLE BankInfo (
        BankId INT IDENTITY(1,1) PRIMARY KEY,
        BankName NVARCHAR(100) NOT NULL,
        ApiProviderCode NVARCHAR(50) NOT NULL UNIQUE,
        LogoUrl NVARCHAR(500) NULL,
        CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    )
END;
GO

-- =================================================================================
-- TABLA: Categorias
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Category')
BEGIN
    CREATE TABLE Category (
        CategoryId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NULL FOREIGN KEY
            REFERENCES Users(UserId) ON DELETE CASCADE,
        CategoryName NVARCHAR(100) NOT NULL,
        CategoryType NVARCHAR(20) NOT NULL CHECK (CategoryType IN ('Ingreso', 'Gasto', 'Transferencia')),
        Icono NVARCHAR(50) NULL,
        FatherCategoryId INT NULL FOREIGN KEY -- Autorreferencia para subcategorías
            REFERENCES Category(CategoryId)
    )
END;
GO

-- =================================================================================
-- TABLA: Conexiones Api (Tokens Open Banking / Integración)
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApiConections')
BEGIN
    CREATE TABLE ApiConections (
        ConectionId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY
            REFERENCES Users(UserId) ON DELETE CASCADE,
        BankId INT NOT NULL FOREIGN KEY (BankId) 
            REFERENCES BankInfo(BankId),
        ExternalItemId NVARCHAR(255) NOT NULL, -- ID del item en la API externa
        AccessToken NVARCHAR(MAX) NOT NULL,
        RefreshToken NVARCHAR(MAX) NULL,
        FechaExpiracionToken DATETIME2 NULL,
        TokenStatus NVARCHAR(20) NOT NULL DEFAULT 'Activo', -- 'Activo', 'Expirado', 'Error'
        LastSincronization DATETIME2 NULL,
        CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    )
END;
GO

-- =================================================================================
-- TABLA: Cuentas
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
BEGIN
    CREATE TABLE Accounts (
        AccountId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY
            REFERENCES Users(UserId) ON DELETE CASCADE,
        BankId INT NOT NULL FOREIGN KEY (BankId) 
            REFERENCES BankInfo(BankId),
        ConectionId INT NOT NULL FOREIGN KEY
            REFERENCES ApiConections(ConectionId),
        ExternalAccountId NVARCHAR(255) NULL, -- ID devuelto por la API bancaria
        AccountName NVARCHAR(100) NOT NULL,
        AccountType NVARCHAR(50) NOT NULL, -- 'Ahorros', 'Corriente', 'TarjetaCredito', 'Inversion'
        Currency CHAR(3) NOT NULL DEFAULT 'EUR',
        Balance DECIMAL(18,4) NOT NULL CONSTRAINT DF_Cuentas_Saldo DEFAULT 0.0000,
        AccountStatus BIT NOT NULL DEFAULT 1,
        CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    )
END;
GO

-- =================================================================================
-- TABLA: Transacciones
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions (
        TransactionId BIGINT IDENTITY(1,1) PRIMARY KEY,
        AccountId INT NOT NULL FOREIGN KEY
            REFERENCES Accounts(AccountId),
        UserId INT NOT NULL FOREIGN KEY
            REFERENCES Users(UserId),
        CategoryId INT NULL FOREIGN KEY
            REFERENCES Category(CategoryId) ON DELETE SET NULL,
        ExternalTransactionId NVARCHAR(255) NULL, -- ID único de la API para prevenir duplicados
        Amount DECIMAL(18,4) NOT NULL,
        Currency CHAR(3) NOT NULL DEFAULT 'EUR',
        TransactionDate DATETIME2 NOT NULL,
        TransactionDescription NVARCHAR(500) NOT NULL,
        TransactionStatus NVARCHAR(20) NOT NULL CONSTRAINT DF_Transacciones_Estado DEFAULT 'Completado', -- 'Pendiente', 'Completado'
        IsIgnored BIT NOT NULL CONSTRAINT DF_Transacciones_EsIgnorada DEFAULT 0, -- Útil para ocultar del presupuesto
        CreationDate DATETIME2 NOT NULL CONSTRAINT DF_Transacciones_Fecha DEFAULT SYSDATETIME(),
    )
END;
GO

-- =================================================================================
-- TABLA: Transferencias Internas
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transfers')
BEGIN
    CREATE TABLE Transfers (
        TransferId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY
            REFERENCES Users(UserId),
        TransferOriginId BIGINT NOT NULL FOREIGN KEY
            REFERENCES Transactions(TransactionId),
        TransferEndId BIGINT NOT NULL FOREIGN KEY
            REFERENCES Transactions(TransactionId),
        Amount DECIMAL(18,4) NOT NULL,
        CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    )
END;
GO

-- =================================================================================
-- TABLA: Presupuestos
-- =================================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Budgets')
BEGIN
    CREATE TABLE Budgets (
        BudgetId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL FOREIGN KEY
            REFERENCES Users(UserId),
        CategoryId INT NULL FOREIGN KEY
            REFERENCES Category(CategoryId),
        MontoLimite DECIMAL(18,4) NOT NULL,
        Periodo NVARCHAR(20) NOT NULL CONSTRAINT CHK_Presupuestos_Periodo CHECK (Periodo IN ('Mensual', 'Anual', 'Personalizado')),
        FechaInicio DATE NOT NULL,
        FechaFin DATE NOT NULL,
        FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Presupuestos_Fecha DEFAULT SYSDATETIME()
    )
END;
GO

-- ============================================================================
-- ÍNDICES RECOMENDADOS PARA RENDIMIENTO
-- ============================================================================

-- Búsquedas rápidas de transacciones por usuario y ordenadas por fecha (Feed Principal)
CREATE NONCLUSTERED INDEX IX_Transactions_User_CreationDate
ON Transactions (UserId, CreationDate DESC)
INCLUDE (Amount, CategoryId, AccountId, TransactionDescription);
GO

-- Filtro de transacciones por cuenta
CREATE NONCLUSTERED INDEX IX_Transactions_Account_TransactionDate
ON Transactions (AccountId, TransactionDate DESC);
GO

-- Cálculo rápido de presupuestos por categoría y rango de fechas
CREATE NONCLUSTERED INDEX IX_Transactions_Category_TransactionDate
ON Transactions (CategoryId, TransactionDate)
INCLUDE (Amount)
WHERE IsIgnored = 0;
GO

-- Búsqueda de cuentas activas por usuario
CREATE NONCLUSTERED INDEX IX_Accounts_User
ON Accounts (UserId)
WHERE AccountStatus = 1;
GO