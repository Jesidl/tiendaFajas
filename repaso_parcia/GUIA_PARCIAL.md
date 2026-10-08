# GUÍA PARCIAL — Pasos a seguir

> Esta guía solo dice **qué hacer y de dónde copiar**. El código está en los ejercicios:
> - **Tu ejercicio base:** `repaso_parcia/tienda` (Clientes, Productos, Ventas) → plantilla para copiar.
> - **Ejemplos con lógica variada:** `repaso_parcia/ejemplos/` → ver `ejemplos/README.md` (enunciados + catálogo de patrones).

| Ejercicio | Úsalo para copiar… |
|---|---|
| `tienda` | Estructura completa, CRUD básico, cálculo de total, `Parcial` |
| `ejemplos/ejemplo3_clinica` | Validaciones, estados, choque de horario, borrado lógico, consultas con parámetro |
| `ejemplos/ejemplo4_biblioteca` | Modificar otra tabla (stock), fechas límite, multas, acciones tipo `Devolver/{id}` |
| `ejemplos/ejemplo5_banco` | Relación en cadena, `ThenInclude`, campos protegidos, código generado, DTO de reporte |
| `ejemplos/ejemplo6_universidad` | Muchos a muchos, cupos (`Count`), límites (`Sum`), `switch`, reportes `GroupBy` |
| `ejemplos/ejemplo7_parqueadero` | Cálculo con tiempo/horas, topes, asignación automática, mover entre registros |

---

## Antes de empezar (este PC tiene 6 GB de RAM)
- Cerrar navegadores, VS Code y todo lo que no se use. Con poca RAM, SQL Server tarda en aceptar conexiones y aparece *"Valor de tiempo de espera caducado"*.
- Dejar abierto solo Visual Studio y **un** navegador (o probar con el archivo `.http`).
- Ejecutar siempre con **Ctrl+F5** (sin depuración: consume menos memoria y no se detiene en cada `throw`).

## 0. Plan de tiempo (~1 hora)

| Min | Paso |
|---|---|
| 0–5 | Leer enunciado. Subrayar tablas, campos, tipos y **reglas** |
| 5–10 | Paso 1: estructura |
| 10–20 | Pasos 2–5: entidades, DatosGenerales, IConexion, Conexion |
| 20–25 | Paso 6: migración |
| 25–40 | Pasos 7–8: interfaces, aplicaciones y **reglas de negocio** |
| 40–50 | Paso 9: Program.cs y controllers |
| 50–60 | Paso 10: probar + checklist final |

---

## 1. Estructura (Visual Studio)

1. Crear proyecto → **Solución en blanco**.
2. Agregar → Nuevo proyecto → **Biblioteca de clases (C#)** `libreria_X` (.NET 10). Borrar `Class1.cs`.
3. NuGet en la librería (misma versión 10.x): `Microsoft.EntityFrameworkCore.SqlServer` y `Microsoft.EntityFrameworkCore.Tools`.
4. Carpetas: `entidades`, `interfaces`, `implementaciones`, `nucleo`.
5. Agregar → Nuevo proyecto → **ASP.NET Core Web API** `servicios_X` (.NET 10, "Usar controladores" ✔, OpenAPI ✘).
6. Borrar `WeatherForecast.cs` y su controller.
7. `servicios_X` → Agregar → **Referencia de proyecto** → `libreria_X`.
8. Compilar (**Ctrl+Shift+B**).

## 2. Entidades → copiar de `tienda/libreria_tienda/entidades`
- Tabla maestra → `Clientes.cs`. Tabla con las FK → `Ventas.cs`.
- Relación en cadena → `ejemplo5_banco/.../entidades/Cuentas.cs` (tiene FK **y** lista).
- Campos que pueden quedar vacíos (`DateTime?`, `decimal?`) → `ejemplo4_biblioteca/.../Prestamos.cs`, `ejemplo6_universidad/.../Matriculas.cs`.
- Revisar: `public class`, `Id` con I mayúscula, `string?`, dinero en `decimal`, `using ...DataAnnotations.Schema` en la tabla con FK.

## 3. `nucleo/DatosGenerales.cs` → copiar de `tienda`
- Cambiar nombre de la base. En este PC el servidor es `localhost\\SQLEXPRESS`.
- Agregar al final de la cadena **`Connect Timeout=60;`** → si SQL tarda en responder, la API espera 60 s en vez de fallar a los 15 s.

## 4. `interfaces/IConexion.cs` → copiar de `tienda`
- Un `DbSet` por tabla. Dejar `Entry<T>` y `SaveChanges()`.

## 5. `implementaciones/Conexion.cs` → copiar de `tienda`
- Mismos `DbSet` que la interfaz. Conservar el `??` con `DatosGenerales` (sin eso falla `Update-Database`).

## 6. Migración
1. `libreria_X` → **Establecer como proyecto de inicio**.
2. Herramientas → NuGet → **Consola del Administrador de paquetes** → Proyecto predeterminado = `libreria_X`.
3. `Add-Migration migration1`
4. `Update-Database`
5. Verificar en SQL Server: tablas + llaves foráneas.
6. **Desactivar AUTO_CLOSE** (SQL Express lo activa por defecto y cierra/reabre la base en cada conexión → lentitud y timeouts). En la misma Consola del Administrador de paquetes, justo después de `Update-Database`:
   `sqlcmd -S "localhost\SQLEXPRESS" -E -C -Q "ALTER DATABASE db_X SET AUTO_CLOSE OFF"`
   (o en SQL Server Management Studio → Nueva consulta → `ALTER DATABASE db_X SET AUTO_CLOSE OFF`).
7. Volver a poner `servicios_X` como proyecto de inicio.
- Si cambias una entidad después: `Add-Migration migration2` + `Update-Database`.
- Si la migración salió mal y no se aplicó: `Remove-Migration`.
- Si piden el script SQL a mano: ver `Script.sql` de cualquier ejemplo.

## 7. Interfaces de aplicación → copiar de `tienda/libreria_tienda/interfaces`
- Los 4 métodos (Insertar, Consultar, Actualizar, Borrar).
- **Todo método extra** que pida el enunciado también va aquí (si no, el controller no lo ve).

## 8. Aplicaciones (la lógica) → copiar de `tienda` y aplicar los patrones
- Tablas sencillas → `ClientesAplicacion.cs` de `tienda`.
- Tabla con FK → `VentasAplicacion.cs` de `tienda` (Include + limpiar ciclos + cálculo).
- **Por cada regla del enunciado:** buscar la frase en la tabla **"A. Traducir el enunciado a un patrón"** de `ejemplos/README.md` y copiar del archivo que indica.
- Orden dentro de cada método: validar → valores por defecto/calcular → otras tablas → Add/Remove/Modified → **un solo** SaveChanges → return.
- Si modificas un registro consultado (stock, saldo, estado): **siempre** marcarlo Modified (ver `PrestamosAplicacion.cs` de biblioteca).

## 9. Servicio web
1. `Program.cs` → copiar de `tienda/servicios_tienda` (incluye `IgnoreCycles`).
2. Un controller por tabla → copiar `ClientesController.cs` de `tienda` y renombrar.
3. Endpoint `Parcial` en el controller de la tabla con FK → `VentasController.cs` de `tienda`.
4. Endpoints extra (por id, por filtro, acciones, reportes) → tabla **"C. Rutas en el controller"** de `ejemplos/README.md`.
5. Cada GET extra necesita **ruta propia** (`[HttpGet("Nombre")]`).

## 10. Probar
- **Ctrl+F5** (sin depuración; F5 se detiene en cada `throw`).
- Navegador para GET: `/Tabla/Parcial`, `/Tabla`. Puerto en `Properties/launchSettings.json`.
- POST/PUT/DELETE: archivo `.http` → copiar formato de `tienda/servicios_tienda/servicios_tienda.http` o de cualquier ejemplo.

---

## Errores típicos

| Error | Solución |
|---|---|
| `CS0246 ... no se encontró` | Namespace/using mal renombrado tras copiar. Ctrl+H por archivo |
| *inaccesible por su nivel de protección* | Clase `internal` → `public` |
| *no implementa el miembro de interfaz* | DbSet o método distinto entre interfaz y clase |
| *requires a primary key* | Falta `Id` con I mayúscula |
| `ForeignKey` no encontrado | Falta el using de `DataAnnotations.Schema` (Ctrl + .) |
| *Add-Migration no se reconoce* | Falta paquete Tools o proyecto predeterminado incorrecto |
| *Unable to create a DbContext* | El proyecto de inicio no es la librería |
| *No se ha inicializado la propiedad ConnectionString* | Falta el `??` en `Conexion` |
| *A network-related error* | Servidor mal escrito en DatosGenerales |
| *Valor de tiempo de espera caducado* / *Login timeout expired* (con "Tras el inicio de sesión completo=14000") | SQL lento: `AUTO_CLOSE OFF` (paso 6.6), `Connect Timeout=60` (paso 3) y cerrar programas para liberar RAM |
| *Metadata file ... .dll could not be found* | No es el error real: la librería no compiló. Filtro de la lista en **Build Only** o ver ventana Salida |
| La petición se queda cargando | VS está detenido en una excepción (F5) → Shift+F5 y ejecutar con Ctrl+F5 |
| *A possible object cycle was detected* | Falta `IgnoreCycles` o limpiar ciclos en Consultar |
| *AmbiguousMatchException* | Dos `[HttpGet]` sin ruta distinta |
| *file is being used by another process* | La API sigue corriendo → Shift+F5 |
| Un cambio en otra tabla no se guarda | Falta marcarlo Modified |
| Cambios no aparecen | Guardar todo: **Ctrl+Shift+S** |

---

## Checklist final

- [ ] Entidades `public`, `Id`, tipos correctos, FK con `[ForeignKey]`
- [ ] Ningún archivo con nombres del ejercicio copiado (namespace, clases, tablas)
- [ ] IConexion y Conexion con los mismos DbSet
- [ ] Interfaces y aplicaciones; métodos extra también en la interfaz
- [ ] **Cada regla del enunciado implementada** (Insertar **y** Actualizar)
- [ ] Migración creada y aplicada; tablas y FK visibles en SQL Server
- [ ] `servicios_X` como proyecto de inicio, Program.cs con `IgnoreCycles`
- [ ] Controllers completos + `Parcial` + endpoints extra con ruta propia
- [ ] Probado `Parcial`, listado y un POST con solo Ids
- [ ] 0 errores al compilar
