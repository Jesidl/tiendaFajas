/* ---------------------------------------------------------------------------
   Paso 4. Inicio de sesion en SQL Server para la identidad del grupo de
   aplicaciones del servicio.

   Este es el paso que casi todo el mundo se salta, y el sintoma no ayuda: el
   sitio arranca bien, responde, y toda consulta falla con "Error
   Comunicacion" en el front.

   El motivo. La cadena de conexion usa Integrated Security=True, es decir
   "conectate a SQL Server como el usuario de Windows que esta ejecutando este
   proceso". En desarrollo ese usuario es la persona que ejecuta dotnet run, y
   tiene permisos. Bajo IIS el proceso corre como IIS APPPOOL\<grupo>, una
   cuenta virtual que SQL Server no conoce.

   Solo el SERVICIO necesita esto. El front no abre ninguna conexion: aunque
   arrastre Entity Framework por dependencia transitiva, nunca lo usa.

   Ejecutar como un inicio de sesion con rol sysadmin, desde SSMS o con:
     sqlcmd -S "localhost\SQLEXPRESS" -E -i 4-permisos-sql.sql
   --------------------------------------------------------------------------- */

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals
               WHERE name = N'IIS APPPOOL\tiendaFajas-servicio')
BEGIN
    CREATE LOGIN [IIS APPPOOL\tiendaFajas-servicio] FROM WINDOWS;
    PRINT 'inicio de sesion creado';
END
ELSE
    PRINT 'el inicio de sesion ya existia';
GO

USE [db_tienda_fajas];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals
               WHERE name = N'IIS APPPOOL\tiendaFajas-servicio')
BEGIN
    CREATE USER [IIS APPPOOL\tiendaFajas-servicio]
        FOR LOGIN [IIS APPPOOL\tiendaFajas-servicio];
    PRINT 'usuario de base de datos creado';
END
ELSE
    PRINT 'el usuario de base de datos ya existia';
GO

/* Lectura y escritura sobre las tablas, nada mas. No se le da db_owner: el
   servicio no crea ni modifica estructuras, solo filas. */
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\tiendaFajas-servicio];
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\tiendaFajas-servicio];
GO

PRINT '';
PRINT '--- comprobacion ---';

SELECT  p.name          AS usuario,
        r.name          AS rol
FROM    sys.database_role_members m
JOIN    sys.database_principals r ON r.principal_id = m.role_principal_id
JOIN    sys.database_principals p ON p.principal_id = m.member_principal_id
WHERE   p.name = N'IIS APPPOOL\tiendaFajas-servicio';
GO
