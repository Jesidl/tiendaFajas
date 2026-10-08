/*
-- Muchos a muchos con tabla intermedia que tiene datos propios
CREATE DATABASE db_universidad;
GO
USE db_universidad;
GO

CREATE TABLE [Estudiantes] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Estrato] INT NOT NULL,
    [Activo] BIT NOT NULL
);

CREATE TABLE [Cursos] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Creditos] INT NOT NULL,
    [Cupo] INT NOT NULL,
    [ValorCredito] DECIMAL(18, 2) NOT NULL
);

CREATE TABLE [Matriculas] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Estudiante] INT NOT NULL REFERENCES [Estudiantes]([Id]),
    [Curso] INT NOT NULL REFERENCES [Cursos]([Id]),
    [Periodo] NVARCHAR(10) NOT NULL,
    [Fecha] DATETIME2 NOT NULL,
    [Valor] DECIMAL(18, 2) NOT NULL,
    [Descuento] DECIMAL(18, 2) NOT NULL,
    [Total] DECIMAL(18, 2) NOT NULL,
    [Nota] DECIMAL(18, 2) NULL,              -- decimal? en C#
    [Estado] NVARCHAR(20) NOT NULL
);
*/
