/*
CREATE DATABASE db_notas;
GO 
USE db_notas;
GO 

CREATE TABLE [Materias] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(50) NOT NULL,
    [Programa] NVARCHAR(20) NOT NULL,
);

CREATE TABLE [Estudiantes] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nombres] NVARCHAR(200) NOT NULL,
    [Fecha] SMALLDATETIME NOT NULL,
    [Estado] BIT NOT NULL,
);

CREATE TABLE [Notas] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Materia] INT NOT NULL REFERENCES [Materias]([Id]),
    [Estudiante] INT NOT NULL REFERENCES [Estudiantes]([Id]),
    [Nota1] DECIMAL(10, 2) NOT NULL,
    [Nota2] DECIMAL(10, 2) NOT NULL,
    [Nota3] DECIMAL(10, 2) NOT NULL,
    [Nota4] DECIMAL(10, 2) NOT NULL,
    [Nota5] DECIMAL(10, 2) NOT NULL,
    [NotaFinal] DECIMAL(10, 2) NOT NULL,
);
*/