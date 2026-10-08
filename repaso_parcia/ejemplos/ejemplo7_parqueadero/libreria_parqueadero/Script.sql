/*
CREATE DATABASE db_parqueadero;
GO
USE db_parqueadero;
GO

CREATE TABLE [Vehiculos] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Placa] NVARCHAR(6) NOT NULL,
    [Tipo] NVARCHAR(10) NOT NULL,
    [Propietario] NVARCHAR(100) NULL,
    [Mensualidad] BIT NOT NULL
);

CREATE TABLE [Celdas] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Codigo] NVARCHAR(10) NOT NULL,
    [Tipo] NVARCHAR(10) NOT NULL,
    [Ocupada] BIT NOT NULL
);

CREATE TABLE [Registros] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Vehiculo] INT NOT NULL REFERENCES [Vehiculos]([Id]),
    [Celda] INT NOT NULL REFERENCES [Celdas]([Id]),
    [Entrada] DATETIME2 NOT NULL,
    [Salida] DATETIME2 NULL,
    [Minutos] INT NOT NULL,
    [Valor] DECIMAL(18, 2) NOT NULL,
    [Cerrado] BIT NOT NULL
);
*/
