/*
CREATE DATABASE db_biblioteca;
GO
USE db_biblioteca;
GO

CREATE TABLE [Libros] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Isbn] NVARCHAR(20) NOT NULL,
    [Titulo] NVARCHAR(150) NOT NULL,
    [Autor] NVARCHAR(100) NOT NULL,
    [Ejemplares] INT NOT NULL,
    [Disponibles] INT NOT NULL
);

CREATE TABLE [Lectores] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Documento] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Correo] NVARCHAR(100) NOT NULL,
    [Activo] BIT NOT NULL
);

CREATE TABLE [Prestamos] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Libro] INT NOT NULL REFERENCES [Libros]([Id]),
    [Lector] INT NOT NULL REFERENCES [Lectores]([Id]),
    [FechaPrestamo] DATETIME2 NOT NULL,
    [FechaLimite] DATETIME2 NOT NULL,
    [FechaDevolucion] DATETIME2 NULL,          -- NULL: aun no devuelto (DateTime? en C#)
    [DiasRetraso] INT NOT NULL,
    [Multa] DECIMAL(18, 2) NOT NULL,
    [Devuelto] BIT NOT NULL
);
*/
