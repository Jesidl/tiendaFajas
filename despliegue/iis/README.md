# Publicación local en IIS

Deja los dos procesos de la aplicación hospedados en IIS, de modo que arranquen
con Windows y vivan en una dirección fija, sin necesidad de `dotnet run` ni de
un editor abierto.

| | Sitio | Puerto |
|---|---|---|
| API REST | `tiendaFajas-servicio` | **5250** |
| Front Razor Pages | `tiendaFajas-front` | **5150** |

Los puertos son los mismos que en desarrollo, y eso es deliberado: el front
tiene la dirección del servicio en
[`Servidor.cs`](../../backend/presentacion_tiendaFajas/nucleo/Servidor.cs), y el
`src` de las imágenes apunta ahí. Manteniéndolos, pasar a IIS no exige ningún
cambio de código.

---

## Pasos

| # | Script | Consola |
|---|---|---|
| 1 | `1-instalar-iis.ps1` | **administrador** |
| 2 | `2-publicar.ps1` | normal |
| 3 | `3-crear-sitios.ps1` | **administrador** |
| 4 | `4-permisos-sql.sql` | `sysadmin` de SQL Server |
| 5 | `5-verificar.ps1` | normal |

```powershell
# 1 y 3, en una consola abierta con "Ejecutar como administrador"
cd E:\programacion\proyectos\tiendaFajas\despliegue\iis
.\1-instalar-iis.ps1

# 2 y 5, en una consola normal
.\2-publicar.ps1

# 4
sqlcmd -S "localhost\SQLEXPRESS" -E -i 4-permisos-sql.sql
```

Entre el paso 1 y el 2 hay que instalar a mano el **ASP.NET Core Hosting
Bundle**, desde <https://dotnet.microsoft.com/download/dotnet/10.0> (columna
Windows, enlace *Hosting Bundle*). El script 1 lo detecta y se detiene con el
enlace si falta.

> **El orden importa.** Si el Hosting Bundle se instala *antes* de activar IIS,
> no registra su módulo y los sitios responden 500.19. Se arregla con
> *Panel de control → Programas → Reparar* sobre el Hosting Bundle.

Al terminar, la aplicación está en <http://localhost:5150>.

---

## Las tres cosas que fallan

### 1. SQL Server no conoce a la identidad del sitio

Es el fallo más común y el síntoma engaña: el sitio arranca, responde, y **toda
consulta falla** con `Error Comunicacion` en pantalla.

La cadena de conexión usa `Integrated Security=True`, o sea *«conéctate a SQL
Server como el usuario de Windows que ejecuta este proceso»*. En desarrollo ese
usuario eres tú, y tienes permisos. Bajo IIS el proceso corre como
`IIS APPPOOL\tiendaFajas-servicio`, una cuenta virtual que SQL Server no conoce.

Lo resuelve el **paso 4**, que le crea el inicio de sesión y le da
`db_datareader` y `db_datawriter` — nada más: el servicio escribe filas, no
estructuras.

Solo el servicio lo necesita. El front no abre ninguna conexión, aunque arrastre
Entity Framework por dependencia transitiva.

### 2. El grupo de aplicaciones con la versión de CLR equivocada

El grupo tiene que estar en **«Sin código administrado»**. ASP.NET Core no usa
el CLR que IIS carga por omisión: su módulo lanza un proceso de `dotnet` aparte.
Con el valor por omisión (v4.0), el sitio responde 500.30 o 502.5.

El paso 3 ya lo deja así (`managedRuntimeVersion = ''`).

### 3. Un 500.30 que no dice nada

Cuando una aplicación de ASP.NET Core no arranca bajo IIS, el navegador muestra
*«500.30 - ASP.NET Core app failed to start»* sin excepción, archivo ni línea.

El paso 2 activa el registro de la salida estándar, así que el motivo real queda
escrito en:

```
E:\publicado\tiendaFajas\servicio\logs\stdout_*.log
E:\publicado\tiendaFajas\front\logs\stdout_*.log
```

Ese registro se desactiva cada vez que se publica, porque `dotnet publish`
regenera el `web.config`. El script 2 lo vuelve a activar.

---

## Después de cambiar código

```powershell
.\2-publicar.ps1
```

No hace falta repetir los pasos 1, 3 y 4. El script detiene los procesos de
desarrollo que bloquearían los DLL, pero **IIS también los bloquea**: si la
publicación falla con `MSB3027`, detenga los sitios primero.

```powershell
# como administrador
Import-Module WebAdministration
Stop-Website 'tiendaFajas-servicio'; Stop-Website 'tiendaFajas-front'
# ... publicar ...
Start-Website 'tiendaFajas-servicio'; Start-Website 'tiendaFajas-front'
```

---

## Notas

- La carpeta publicada (`E:\publicado\tiendaFajas`) está **fuera del
  repositorio** a propósito: IIS no debe servir el árbol de código fuente, donde
  están el `Script.sql` y los archivos de proyecto.
- El *Default Web Site* de IIS, en el puerto 80, no se toca y no estorba.
- Los sitios usan `AlwaysRunning` y `preloadEnabled`, así que el proceso se
  carga al arrancar Windows en vez de en la primera visita.
- Volver a ejecutar el paso 3 es seguro: borra y recrea los sitios y los grupos.
