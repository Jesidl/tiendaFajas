/*
-- Relacion en CADENA: Clientes -> Cuentas -> Movimientos
CREATE DATABASE db_banco;
GO
USE db_banco;
GO

CREATE TABLE [Clientes] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Documento] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Telefono] NVARCHAR(20) NULL,
    [Activo] BIT NOT NULL
);

CREATE TABLE [Cuentas] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Numero] NVARCHAR(20) NOT NULL,
    [Cliente] INT NOT NULL REFERENCES [Clientes]([Id]),
    [Tipo] NVARCHAR(20) NOT NULL,
    [Saldo] DECIMAL(18, 2) NOT NULL,
    [FechaApertura] DATETIME2 NOT NULL,
    [Activa] BIT NOT NULL
);

CREATE TABLE [Movimientos] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Cuenta] INT NOT NULL REFERENCES [Cuentas]([Id]),
    [Tipo] NVARCHAR(20) NOT NULL,
    [Valor] DECIMAL(18, 2) NOT NULL,
    [Impuesto] DECIMAL(18, 2) NOT NULL,
    [SaldoAnterior] DECIMAL(18, 2) NOT NULL,
    [SaldoNuevo] DECIMAL(18, 2) NOT NULL,
    [Fecha] DATETIME2 NOT NULL,
    [Descripcion] NVARCHAR(200) NULL
);
*/
