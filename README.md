# tiendaFajas

Sistema de gestión para una tienda de fajas y prendas de compresión: catálogo de
productos con variantes por talla y color, inventario por sucursal, ventas,
pedidos a proveedores, medidas de clientes, y un módulo de usuarios con roles y
bitácora de auditoría.

Construido en **C# / .NET 10** con **Entity Framework Core** sobre **SQL Server**,
una **API REST** en ASP.NET Core y un front en **Razor Pages**.

---

## Qué hace

| Módulo | Contenido |
|---|---|
| **Catálogos** | Cargos, sucursales, categorías, marcas, materiales, tallas, colores y métodos de pago |
| **Personas** | Clientes, proveedores, empleados y medidas corporales por cliente |
| **Productos** | Productos con sus fotografías, variantes (combinación de talla y color con su SKU) e inventario por sucursal |
| **Ventas** | Ventas con detalle por variante y pagos por método |
| **Compras** | Pedidos a proveedores con su detalle |
| **Seguridad** | Usuarios, roles, permisos por módulo y auditoría de toda escritura |

Son **25 tablas**, y cada una tiene su API REST completa y su pantalla de
administración: insertar, actualizar, consultar, buscar y borrar.

---

## Arquitectura

Cinco proyectos distribuidos en **dos procesos independientes** que se comunican
por HTTP. El front nunca habla con la base de datos: solo manda y recibe JSON.

```
                    proceso 1 (frontend)              proceso 2 (backend)
                 ┌──────────────────────┐          ┌──────────────────────┐
  navegador ───► │  front_tiendaFajas   │          │ servicio_tiendaFajas │
                 │    (Razor Pages)     │          │   (API REST)         │
                 │          │           │  HTTP    │          │           │
                 │          ▼           │ ───────► │          ▼           │
                 │ presentacion_        │   JSON   │ aplicacion_          │
                 │   tiendaFajas        │          │   tiendaFajas        │
                 │  (cliente HTTP)      │          │  (negocio + datos)   │
                 └──────────────────────┘          └──────────┬───────────┘
                                                              │ EF Core
                                                              ▼
                                                        SQL Server
```

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `backend/aplicacion_tiendaFajas` | Librería | Entidades, reglas de negocio, `DbContext` y acceso a datos |
| `backend/servicio_tiendaFajas` | Web API | 25 controladores, 128 endpoints |
| `backend/presentacion_tiendaFajas` | Librería | Cliente HTTP y una fachada por entidad |
| `frontend/front_tiendaFajas` | Razor Pages | 24 pantallas de administración, el login y la galería de imágenes |
| `backend/pruebas_tiendaFajas` | MSTest | 64 pruebas |

Las dependencias van en cadena, nunca cruzadas:

```
front ──► presentacion ──► aplicacion
servicio ──────────────► aplicacion
pruebas ──► presentacion + aplicacion
```

**Por qué dos procesos y no uno.** El límite entre `front` y `servicio` es un
límite de proceso real, no una separación de carpetas: cada lado se compila, se
arranca y se despliega por su cuenta, y pueden vivir en máquinas distintas. Las
carpetas `backend/` y `frontend/` reflejan ese límite, de modo que desplegar una
mitad no obliga a reconstruir la otra.

---

## Modelo de datos

25 tablas con integridad referencial declarada. El script completo está en
[`backend/aplicacion_tiendaFajas/Script.sql`](backend/aplicacion_tiendaFajas/Script.sql).

```
Cargos ──┐
         ├──► Empleados ──┬──► Ventas ──┬──► Detalles_Ventas
Sucursales ──┐            │             └──► Pagos ──► Metodos_Pago
             │            └──► Pedidos ──► Detalles_Pedidos
             │                   │
Clientes ──┬─┼───────────────────┘ (vía Proveedores)
           └─┴──► Medidas

Categorias ─┐           ┌──► Imagenes
Marcas ─────┼──► Productos ──► Variantes ──┬──► Inventarios
Materiales ─┘         Tallas ──┘           ├──► Detalles_Ventas
                      Colores ──┘          └──► Detalles_Pedidos

Roles ──┬──► Usuarios
        └──► Permisos                      Auditorias (sin FK)
```

Las claves foráneas se modelan con dos miembros: el escalar que persiste y la
navegación que EF Core resuelve con `Include()`.

```csharp
public int Categoria { get; set; }

[ForeignKey("Categoria")]
public Categorias? _Categoria { get; set; }
```

---

## Seguridad

- Las contraseñas se derivan con **PBKDF2-HMAC-SHA256**, 210 000 iteraciones,
  sal aleatoria de 16 bytes y hash de 32, y se guardan como `sal.hash`. La
  verificación usa comparación en tiempo constante
  (`CryptographicOperations.FixedTimeEquals`), que no filtra cuántos bytes
  coincidieron.
- La derivación ocurre **solo en la capa de aplicación**. El front nunca calcula
  ni ve un hash, y la contraseña en claro no sale del proceso que la recibe.
- El login no distingue entre correo inexistente y contraseña equivocada: el
  mismo mensaje para los dos casos, para no revelar qué cuentas existen.
- La sesión se guarda del lado del servidor con cookie `HttpOnly`, 30 minutos de
  inactividad. Cada página verifica la sesión en **todos** sus manejadores, no
  solo al cargar.

---

## Auditoría

Toda escritura queda registrada en la tabla `Auditorias` con la entidad, la
llave, la operación, el usuario, la fecha y los campos afectados.

No está implementada repitiendo código en las 24 clases de negocio, sino
**interceptando `SaveChanges()`** y recorriendo el `ChangeTracker`. Tres
consecuencias de esa decisión:

1. Cubre `Insertar`, `Actualizar` y `Borrar` de toda entidad, incluidas las que
   se agreguen después, sin tocar ninguna clase de negocio.
2. La fila y su rastro se escriben **dentro de la misma transacción**, así que no
   puede quedar un cambio sin registrar ni un registro sin cambio.
3. `Auditorias` **no tiene ninguna clave foránea**, a propósito: un registro de
   auditoría tiene que sobrevivir al borrado de la fila que describe.

El usuario responsable viaja del front al servicio en la cabecera `X-Usuario`,
transportado con `AsyncLocal` en los dos lados para que dos sesiones atendidas a
la vez por el mismo proceso no se mezclen. Las escrituras sin sesión quedan
atribuidas a `sistema`.

La pantalla de `Auditorias` es de **solo lectura**: una bitácora que se puede
editar desde la interfaz no sirve como evidencia.

---

## Imágenes de producto

Cada producto puede tener varias fotografías. Se suben desde su propia pantalla
y se guardan en SQL Server como `VARBINARY(MAX)`.

**El binario no viaja por el canal JSON**, y esa es la decisión que explica todo
lo demás. El cliente HTTP pasa cada respuesta del servicio por una cadena de
reemplazos de texto que, entre otras cosas, sustituye `null` por una cadena
vacía. Una imagen en base64 son miles de caracteres del alfabeto `A-Za-z0-9+/`,
así que contiene esa subcadena casi con seguridad: llegaría corrupta, y con otro
archivo el error no se reproduciría. Además base64 infla el tamaño un 33 %.

De ahí salen cuatro consecuencias:

| | |
|---|---|
| La columna `Contenido` lleva `[JsonIgnore]` | Nunca entra ni sale en JSON, en ninguna de las dos direcciones |
| Hay un segundo canal, `IArchivos` | Sube el archivo con `multipart/form-data`, el formato que el navegador ya produce para un `<input type="file">` |
| `Imagenes/Ver/{id}` es el único `GET` del servicio | Lo consume el `src` de una etiqueta `<img>`, que no sabe hacer `POST` |
| Es una tabla aparte, no una columna en `Productos` | Un blob dentro de `Productos` se leería en cada consulta de productos aunque nadie pida la foto. Los listados usan una proyección que deja la columna fuera del `SELECT` |

**El tipo del archivo se deduce de sus primeros bytes**, no del `Content-Type`
que declara el cliente, por dos razones. La de fragilidad: un cliente HTTP
cualquiera manda `application/octet-stream` para todo, y un PNG válido sería
rechazado. La de seguridad: ese encabezado lo escribe quien sube el archivo, así
que no prueba nada — si se guardara tal cual, el endpoint que sirve la imagen lo
devolvería al navegador como cabecera, y alguien podría subir un HTML diciendo
que es una imagen para que se sirviera como HTML desde este dominio. La respuesta
lleva además `X-Content-Type-Options: nosniff`, para que el navegador respete el
tipo que declaramos en lugar de adivinarlo.

Se aceptan PNG, JPEG, WEBP y GIF hasta 4 MB. La regla vive en la capa de
aplicación, no en el controlador, para que valga por cualquier vía de entrada.

## Requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) o superior
- **SQL Server** (probado con SQL Server Express) con autenticación de Windows

---

## Puesta en marcha

**1. Crear la base de datos.** Abrir
[`backend/aplicacion_tiendaFajas/Script.sql`](backend/aplicacion_tiendaFajas/Script.sql),
quitar los delimitadores `/* */` y ejecutarlo. Crea `db_tienda_fajas`, las 24
tablas, sus restricciones y los datos semilla.

**2. Ajustar la cadena de conexión** si la instancia no es `localhost\SQLEXPRESS`,
en `backend/aplicacion_tiendaFajas/nucleo/Datosgenerales.cs`.

**3. Arrancar los dos procesos.** Tienen que estar corriendo los dos: el front
sin el servicio muestra `Error Comunicacion`.

```bash
# terminal 1 — API REST en http://localhost:5250
dotnet run --project backend/servicio_tiendaFajas

# terminal 2 — front en http://localhost:5150
dotnet run --project frontend/front_tiendaFajas
```

En Visual Studio hay un perfil de arranque múltiple llamado
**`backend + frontend`** que levanta los dos de una vez.

**4. Entrar** a <http://localhost:5150>.

> El puerto del servicio está escrito en
> `backend/presentacion_tiendaFajas/implementaciones/Comunicaciones.cs`. Si se
> cambia el puerto en `launchSettings.json`, hay que cambiarlo también ahí.

### Publicar en IIS

Para que la aplicación viva en una dirección fija y arranque con Windows, en
lugar de depender de `dotnet run`, hay cinco scripts en
[`despliegue/iis/`](despliegue/iis/) con su propio
[README](despliegue/iis/README.md). Resumen:

```powershell
cd despliegue\iis
.\1-instalar-iis.ps1      # administrador: activa IIS
.\2-publicar.ps1          # compila en Release
.\3-crear-sitios.ps1      # administrador: crea los sitios y grupos
sqlcmd -S "localhost\SQLEXPRESS" -E -i 4-permisos-sql.sql
.\5-verificar.ps1         # comprueba que todo responde
```

Los sitios quedan en los mismos puertos que en desarrollo (5250 y 5150), así que
pasar a IIS no exige ningún cambio de código.

> El paso 4 no es opcional. La cadena de conexión usa `Integrated Security`, y
> bajo IIS el proceso corre como `IIS APPPOOL\tiendaFajas-servicio`, una cuenta
> virtual que SQL Server no conoce. Sin ese inicio de sesión el sitio arranca,
> responde, y toda consulta falla con `Error Comunicacion`.

### Credenciales iniciales

| Correo | Contraseña | Rol |
|---|---|---|
| `admin@tiendafajas.com` | `Admin.123` | Administrador |
| `vendedor@tiendafajas.com` | `Vende.123` | Vendedor |

Los hashes **no** se escriben a mano en el script: se generan desde la
aplicación con `nucleo/Clave.Derivar`.

---

## Pruebas

```bash
dotnet test backend/pruebas_tiendaFajas
```

**64 pruebas** en tres grupos:

| Carpeta | Qué prueba |
|---|---|
| `Repositorios/` | **31**: una por cada una de las 25 tablas, que recorren el ciclo completo contra SQL Server y **leen de vuelta la fila** en cada paso, más 6 del módulo de auditoría. |
| `Comunicaciones/` | **22**: recorren las 25 fachadas por reflexión con un doble en lugar del transporte, verificando que cada operación llame su propia URL y que las validaciones corten antes de salir a la red. Incluye las que garantizan que el binario de una imagen nunca entre al canal JSON. |
| `Dominio/` | **11**: reglas puras, sin base de datos ni red. La derivación de contraseñas y el reconocimiento de imágenes por firma de bytes. |

Dos propiedades del diseño de las pruebas:

- **No dependen de los datos que haya en la base.** 21 de las 25 tablas tienen
  claves foráneas, y una prueba de `Detalles_Ventas` necesita una venta, que
  necesita un cliente y un empleado, que necesitan una sucursal y un cargo.
  `Nucleo/EntidadesNucleo` crea y guarda esa cadena de padres, y la deshace en
  orden inverso al terminar. **La base queda exactamente como estaba.**
- **La que necesita el servicio levantado se declara `Inconclusive`, no fallida,**
  si no lo encuentra. Una prueba que falla porque no se arrancó una dependencia
  no informa de ningún defecto.

---

## Probar la API a mano

Los endpoints son `POST` con cuerpo JSON, **incluidos `Consultar` y
`Buscar`**. Desde la barra del navegador responden 405, porque el navegador manda
`GET`. Hay dos archivos listos en `backend/servicio_tiendaFajas`:

| Archivo | Cómo se usa |
|---|---|
| `tiendaFajas.postman_collection.json` | Se importa en Postman: 25 carpetas, 128 peticiones |
| `servicio_tiendaFajas.http` | Se abre en Visual Studio, o en VS Code con la extensión *REST Client* |

Los cuerpos de `Buscar` traen valores que coinciden con los datos semilla, así
que devuelven filas. Los dos archivos mandan la cabecera `X-Usuario`: sin ella el
rastro de auditoría queda a nombre de `sistema`.

```http
POST http://localhost:5250/Cargos/Buscar
Content-Type: application/json
X-Usuario: admin@tiendafajas.com

{ "Nombre": "Asesora" }
```

---

## Estructura del repositorio

```
tiendaFajas/
├─ backend/
│  ├─ tiendaFajas.slnx                  solución (5 proyectos)
│  ├─ aplicacion_tiendaFajas/
│  │  ├─ entidades/                     24 clases, una por tabla
│  │  ├─ interfaces/                    IConexion + 24 contratos de negocio
│  │  ├─ implementaciones/              Conexion + 24 clases de negocio
│  │  ├─ nucleo/                        Clave, ContextoUsuario, conexión, log
│  │  └─ Script.sql                      DDL + datos semilla
│  ├─ servicio_tiendaFajas/
│  │  ├─ Controllers/                   25 controladores
│  │  └─ tiendaFajas.postman_collection.json
│  ├─ presentacion_tiendaFajas/
│  │  ├─ interfaces/                    IComunicaciones + 24 contratos
│  │  └─ implementaciones/              Comunicaciones, Archivos + 25 fachadas
│  └─ pruebas_tiendaFajas/
│     ├─ Nucleo/                        fábrica de datos y dobles de prueba
│     ├─ Repositorios/                  31 pruebas
│     ├─ Comunicaciones/                22 pruebas
│     └─ Dominio/                       11 pruebas
└─ frontend/
   └─ front_tiendaFajas/
      ├─ Pages/Ventanas/                24 pantallas
      ├─ Pages/Emergentes/              parciales reutilizables
      └─ wwwroot/                       css, js y librerías
```

---

## Decisiones de diseño

**Entidades desconectadas.** El `DbContext` corre con
`QueryTrackingBehavior.NoTracking`, porque las entidades viajan por HTTP y
vuelven como instancias nuevas. Las actualizaciones marcan el estado a mano:

```csharp
var entry = IConexion.Entry<T>(entidad);
entry.State = EntityState.Modified;
IConexion.SaveChanges();
```

**Validación duplicada a propósito.** Las reglas de campos obligatorios y de
`Id` se repiten en el front y en el servicio. El front valida para no gastar una
llamada HTTP; el servicio valida porque no puede confiar en el cliente.

**El hash se guarda en hexadecimal, no en base64.** El cliente HTTP pasa la
respuesta del servicio por una cadena de reemplazos de texto antes de
deserializarla, y uno de ellos sustituye `null` por una cadena vacía. El alfabeto
base64 incluye `n`, `u` y `l`, así que un hash podría contener la subcadena
`null` por casualidad y llegar corrupto, dejando a ese usuario sin poder entrar
nunca más. El alfabeto hexadecimal no puede formar esa palabra. Por el mismo
motivo la columna `Datos` de la auditoría usa `campo=valor; campo=valor` y no
JSON: las llaves y las comillas no sobreviven esa conversión.
Hay una prueba que fija este comportamiento, para que la decisión no se pierda.

**Los combos de claves foráneas se llenan con `Buscar`, no con `Consultar`.**
`Consultar` devuelve un máximo de 5 filas por diseño, y un desplegable con 5
opciones de 200 posibles es un error silencioso. `Buscar` con filtro vacío
devuelve todo.

---

## Limitaciones conocidas

- **El front arrastra la capa de datos.** `presentacion_tiendaFajas` referencia
  `aplicacion_tiendaFajas` para reutilizar las clases de entidad, así que el
  proceso del front termina publicando EF Core, el proveedor de SQL Server y la
  cadena de conexión, aunque no los use. La solución correcta es un sexto
  proyecto que contenga solo las entidades y los contratos, compartido por las
  dos mitades.
- **Los permisos no se aplican todavía.** La tabla `Permisos` define qué puede
  hacer cada rol en cada módulo, pero las pantallas solo verifican que haya
  sesión: cualquier usuario autenticado entra a todo.
- **El servicio tiene que ser alcanzable desde el navegador** para que las
  imágenes se vean: el atributo `src` apunta a `Imagenes/Ver/{id}` del servicio.
  En un despliegue donde el servicio sea interno, hace falta una ruta en el front
  que haga de intermediaria.
- **La cadena de conexión está en el código**, no en `appsettings.json`.
