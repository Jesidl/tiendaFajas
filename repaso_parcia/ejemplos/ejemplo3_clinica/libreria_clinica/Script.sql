/*
-- Equivalente en SQL de la migracion (por si piden el script a mano)
CREATE DATABASE db_clinica;
GO
USE db_clinica;
GO

CREATE TABLE [Pacientes] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Documento] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [FechaNacimiento] DATETIME2 NOT NULL,
    [Subsidiado] BIT NOT NULL,
    [Activo] BIT NOT NULL
);

CREATE TABLE [Medicos] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Nombre] NVARCHAR(100) NOT NULL,
    [Especialidad] NVARCHAR(50) NOT NULL,
    [Tarifa] DECIMAL(18, 2) NOT NULL,
    [Activo] BIT NOT NULL
);

CREATE TABLE [Citas] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Paciente] INT NOT NULL REFERENCES [Pacientes]([Id]),
    [Medico] INT NOT NULL REFERENCES [Medicos]([Id]),
    [Fecha] DATETIME2 NOT NULL,
    [Estado] NVARCHAR(20) NOT NULL,
    [Valor] DECIMAL(18, 2) NOT NULL,
    [Observacion] NVARCHAR(500) NULL
);
*/
