# Guía para el parcial

Esta guía es tu mapa durante el examen. No tiene el código completo, porque ese ya lo tienes en los ejercicios. Aquí solo encuentras **qué hacer, en qué orden y de dónde copiar**.

**¿De dónde copio?**
- **Tu ejercicio `tienda`** (`repaso_parcia/tienda`) es tu plantilla principal. Casi todo sale de ahí.
- **Los ejemplos** (`repaso_parcia/ejemplos/`) sirven por si el profe pide algo más elaborado. En `ejemplos/README.md` está explicado qué tiene cada uno.

| Si necesitas… | Mira en… |
|---|---|
| La estructura completa y lo básico (listar, crear, editar, borrar) | `tienda` |
| Validar datos, manejar estados, cancelar en vez de borrar | `ejemplo3_clinica` |
| Que al guardar algo se actualice otra tabla (por ejemplo, el stock) | `ejemplo4_biblioteca` |
| Tablas encadenadas (A → B → C) o reportes con totales | `ejemplo5_banco` |
| Cupos, límites o promedios | `ejemplo6_universidad` |
| Cálculos con horas y tiempo | `ejemplo7_parqueadero` |

---

## Antes de empezar

Este computador tiene poca memoria (6 GB). Cuando se llena, SQL Server se pone lento y la API empieza a fallar con un mensaje de *"tiempo de espera caducado"*. Para evitarlo:

- Cierra los navegadores, VS Code y cualquier programa que no vayas a usar.
- Deja abiertos solo Visual Studio y **un** navegador.
- Ejecuta la API siempre con **Ctrl+F5**. Así no se queda pausada cada vez que hay un error y gasta menos memoria.

## Cómo repartir la hora

| Minutos | Qué hacer |
|---|---|
| 0 – 5 | Leer el enunciado con calma y subrayar las tablas, sus campos y lo que pide el profe |
| 5 – 10 | Paso 1: crear los proyectos |
| 10 – 20 | Pasos 2 a 5: las clases de las tablas y la conexión |
| 20 – 25 | Paso 6: crear la base de datos con la migración |
| 25 – 40 | Pasos 7 y 8: la parte que guarda y consulta los datos |
| 40 – 50 | Paso 9: el servicio web |
| 50 – 60 | Paso 10: probar todo y revisar la lista final |

---

## Paso 1. Crear los proyectos

1. En Visual Studio crea una **Solución en blanco**.
2. Agrégale una **Biblioteca de clases (C#)** llamada `libreria_X`, con .NET 10. Borra el archivo `Class1.cs` que trae.
3. En la librería instala dos paquetes NuGet, los dos de la **misma versión 10.x**:
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Microsoft.EntityFrameworkCore.Tools`
4. Crea cuatro carpetas: `entidades`, `interfaces`, `implementaciones` y `nucleo`.
5. Agrega un proyecto **ASP.NET Core Web API** llamado `servicios_X`, con .NET 10. Deja marcado "Usar controladores" y desmarca OpenAPI.
6. Borra `WeatherForecast.cs` y su controller, que vienen de ejemplo.
7. En `servicios_X`, agrega una **referencia de proyecto** hacia `libreria_X`. Así el servicio puede usar la librería.
8. Compila con **Ctrl+Shift+B** para confirmar que todo arranca bien.

> Cada vez que termines un paso, guarda todo con **Ctrl+Shift+S**. Varias veces los cambios "desaparecieron" solo porque no estaban guardados.

## Paso 2. Las clases de las tablas (entidades)

Copia de `tienda/libreria_tienda/entidades`:
- Para una tabla sencilla, usa `Clientes.cs` como modelo.
- Para la tabla que conecta a las otras dos (la que tiene las llaves foráneas), usa `Ventas.cs`.

Revisa que:
- Cada clase diga `public class`.
- La llave se llame `Id`, con la I mayúscula.
- Los textos sean `string?` y el dinero sea `decimal`.
- La tabla con llaves foráneas tenga arriba el `using` de `DataAnnotations.Schema`.

Si alguna fecha o valor puede quedar vacío, mira cómo se hizo en `Prestamos.cs` (biblioteca) o en `Matriculas.cs` (universidad).

## Paso 3. La cadena de conexión (`nucleo/DatosGenerales.cs`)

Cópiala de `tienda` y cambia el nombre de la base de datos.
- En este computador el servidor es `localhost\\SQLEXPRESS`.
- Agrega al final **`Connect Timeout=60;`**. Si SQL Server tarda en responder, la API espera hasta 60 segundos en lugar de rendirse a los 15.

## Paso 4. El contrato de la conexión (`interfaces/IConexion.cs`)

Cópialo de `tienda`. Debe tener una línea `DbSet` por cada tabla. No borres `Entry` ni `SaveChanges`: se necesitan para guardar.

## Paso 5. La conexión (`implementaciones/Conexion.cs`)

Cópiala de `tienda`. Los `DbSet` deben llamarse **exactamente igual** que en `IConexion`. Deja el `??` que usa `DatosGenerales`: sin él, la migración no encuentra la base.

## Paso 6. Crear la base de datos (migración)

1. Clic derecho en `libreria_X` → **Establecer como proyecto de inicio**.
2. Abre **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**. Arriba, en "Proyecto predeterminado", elige `libreria_X`.
3. Escribe `Add-Migration migration1` y espera a que termine.
4. Escribe `Update-Database`. Debe terminar diciendo **Done**.
5. Apaga el cierre automático de la base. SQL Express la cierra cada vez que nadie la usa, y eso causa lentitud. En la misma consola escribe:
   ```
   sqlcmd -S "localhost\SQLEXPRESS" -E -C -Q "ALTER DATABASE db_X SET AUTO_CLOSE OFF"
   ```
6. Vuelve a poner `servicios_X` como proyecto de inicio. Si se te olvida, Ctrl+F5 no abre la API.

Si te equivocas:
- **Cambiaste una tabla después de crear la base:** `Add-Migration migration2` y luego `Update-Database`.
- **La migración salió mal y todavía no la aplicaste:** `Remove-Migration`.
- **Te piden el script SQL escrito a mano:** cualquier ejemplo tiene un `Script.sql` de muestra.

## Paso 7. Las interfaces (`interfaces/I...Aplicacion.cs`)

Cópialas de `tienda`. Cada una tiene cuatro acciones: insertar, consultar, actualizar y borrar. Si el profe pide alguna acción extra, también la tienes que agregar aquí; si no, el servicio web no la ve.

## Paso 8. Las clases que hacen el trabajo (`implementaciones/...Aplicacion.cs`)

- Para tablas sencillas, copia `ClientesAplicacion.cs` de `tienda`.
- Para la tabla que conecta a las otras, copia `VentasAplicacion.cs` de `tienda`. Esa ya trae la consulta que une las tablas (el JOIN).

**Si el profe pide reglas** (validar, calcular, no dejar repetir…): busca la frase del enunciado en la tabla **"A. Traducir el enunciado a un patrón"** de `ejemplos/README.md`, y te dice de qué archivo copiar.

Dos consejos:
- Dentro de cada método, el orden casi siempre es: revisar los datos → calcular → guardar → devolver.
- Si cambias un registro que ya existía (el stock, un saldo, un estado), recuerda marcarlo como modificado. Si no, no se guarda y tampoco sale ningún error. El ejemplo está en `PrestamosAplicacion.cs`.

## Paso 9. El servicio web

1. **`Program.cs`:** cópialo de `tienda/servicios_tienda`. Ya trae la línea que evita el error de "ciclo" al mostrar datos relacionados.
2. **Un controller por tabla:** copia `ClientesController.cs` de `tienda` y cambia los nombres.
3. **El endpoint `Parcial`** (si lo piden) va en el controller de la tabla que conecta a las otras. Cópialo de `VentasController.cs`.
4. **Endpoints extra** (buscar por id, filtros, reportes): mira la tabla **"C. Rutas en el controller"** de `ejemplos/README.md`.
5. Si tienes más de un GET en el mismo controller, cada uno necesita su propio nombre de ruta. Si no, la API no sabe cuál usar.

## Paso 10. Probar

Ejecuta con **Ctrl+F5**. El puerto de tu API está en `servicios_X/Properties/launchSettings.json`.

### Consultar (GET): desde el navegador
Escribe la dirección y listo: `http://localhost:PUERTO/Socios`, `http://localhost:PUERTO/Inscripciones`, etc.

### Crear, editar y borrar (POST, PUT, DELETE): desde el archivo `.http`
El navegador solo sirve para consultar. Para lo demás, abre el archivo `servicios_X.http` que viene en el proyecto, borra lo que tiene y pega algo como esto. Este ejemplo es del gimnasio; cambia los nombres, los campos y los números por los de tu ejercicio:

```http
@host = http://localhost:5153

### 1. Ver socios
GET {{host}}/Socios

### 2. CREAR socio (POST) — sin id, la base lo pone sola
POST {{host}}/Socios
Content-Type: application/json

{
  "documento": "2020",
  "nombre": "Maria",
  "estudiante": true,
  "activo": true
}

### 3. EDITAR socio 1 (PUT) — se manda TODO el registro, con su id
PUT {{host}}/Socios
Content-Type: application/json

{
  "id": 1,
  "documento": "10212",
  "nombre": "Hola Actualizado",
  "estudiante": true,
  "activo": true
}

### 4. EDITAR plan 2 (PUT)
PUT {{host}}/Planes
Content-Type: application/json

{
  "id": 2,
  "nombre": "Mensual",
  "precio": 60000,
  "duracionDias": 30
}

### 5. CREAR inscripción (POST) usando los id del socio y del plan
POST {{host}}/Inscripciones
Content-Type: application/json

{
  "socio": 1,
  "plan": 2,
  "fechaInicio": "2026-10-08T00:00:00",
  "fechaFin": "2026-11-07T00:00:00",
  "valor": 60000,
  "descuento": 0,
  "total": 60000
}

### 6. EDITAR inscripción 7 (PUT)
PUT {{host}}/Inscripciones
Content-Type: application/json

{
  "id": 7,
  "socio": 1,
  "plan": 2,
  "fechaInicio": "2026-10-08T00:00:00",
  "fechaFin": "2026-11-07T00:00:00",
  "valor": 60000,
  "descuento": 0,
  "total": 60000
}

### 7. BORRAR inscripción 7 (DELETE) — basta con el id
DELETE {{host}}/Inscripciones
Content-Type: application/json

{
  "id": 7
}

### 8. Ver cómo quedaron las inscripciones
GET {{host}}/Inscripciones
```

Encima de cada `###` aparece un enlace **"Send request"**. Haz clic y la respuesta sale a la derecha. Después de crear, editar o borrar, vuelve a consultar con un GET para confirmar que el cambio quedó en la base.

**Lo que conviene recordar de cada uno:**

| Acción | Qué mandas | Ojo con… |
|---|---|---|
| **Crear (POST)** | Todos los datos **menos el id** | Si mandas un id, falla: lo pone la base |
| **Editar (PUT)** | **Todo** el registro, **con** su id | Lo que no mandes queda vacío. Si olvidas el nombre, se borra |
| **Borrar (DELETE)** | Solo `{ "id": X }` | Nada más |

**Cuidado al borrar una tabla "padre"** (un socio, un plan, un cliente…): se borran también todos los registros que dependen de él. Por ejemplo, si borras un socio, se van todas sus inscripciones. Si solo quieres probar, borra mejor un registro de la tabla que conecta a las otras.

**Antes de usar un id, revisa que exista.** Si un intento de guardar falla, ese número igual queda "gastado", y el siguiente registro sale con el número siguiente. Por eso nunca supongas que el primero es el 1: haz un GET y míralo.

**Si algo sale mal al probar:**
- **400 Bad Request:** el texto JSON está mal escrito. Falta una coma o una comilla, o la fecha no tiene la forma `"2026-10-08T00:00:00"`.
- **500 que menciona FOREIGN KEY:** usaste el id de un socio o un plan que no existe.
- **500 que dice "affected 0 row(s)":** intentaste editar o borrar un id que no está en la tabla.

> **No escribas los datos directamente en la tabla de SQL Server** para probar. Si lo haces, te saltas todo tu código y no sabes si de verdad funciona. Siempre por la API.

---

## Cuando algo falla

| Lo que ves | Qué significa y qué hacer |
|---|---|
| *"...no se encontró"* (CS0246) | Copiaste un archivo y quedó un nombre viejo (por ejemplo, `tienda`). Búscalo con Ctrl+H en ese archivo |
| *"...inaccesible por su nivel de protección"* | A la clase le falta `public` |
| *"...no implementa el miembro de interfaz"* | Algo se llama distinto en `IConexion` y en `Conexion`, o en la interfaz y su clase |
| *"...requires a primary key"* | La llave no se llama `Id` con I mayúscula |
| `ForeignKey` sale en rojo | Falta el `using` de arriba. Pon el cursor encima y presiona **Ctrl + .** |
| *"Add-Migration no se reconoce"* | Falta el paquete Tools, o en la consola está elegido otro proyecto |
| *"Unable to create a DbContext"* | El proyecto de inicio no es la librería |
| *"No se ha inicializado la propiedad ConnectionString"* | Falta el `??` en `Conexion` |
| *"A network-related error"* | El nombre del servidor está mal escrito en `DatosGenerales` |
| *"Valor de tiempo de espera caducado"* o *"Login timeout expired"* | SQL Server está lento. Revisa el paso 6.5 (cierre automático), el paso 3 (`Connect Timeout=60`) y cierra programas |
| *"Metadata file ... .dll could not be found"* | Ese no es el error de verdad: la librería no compiló. Cambia el filtro de la lista de errores a **"Build Only"** o mira la ventana **Salida** |
| La consola dice *"Build failed"* sin más detalle | Compila con Ctrl+Shift+B y mira la lista de errores |
| La página se queda cargando | Visual Studio está pausado en un error. Detén con **Shift+F5** y vuelve a abrir con **Ctrl+F5** |
| *"A possible object cycle was detected"* | Falta la línea especial de `Program.cs`, o limpiar las listas en Consultar |
| *"AmbiguousMatchException"* | Dos GET en el mismo controller sin nombre de ruta distinto |
| *"file is being used by another process"* | La API sigue abierta. Ciérrala con Shift+F5 y vuelve a compilar |
| Íconos rojos al escribir en una tabla de SQL | No es un error, solo falta guardar la fila. Presiona Enter o baja a la fila siguiente |
| Un cambio no se guarda | Falta marcar el registro como modificado, o falta guardar el archivo (**Ctrl+Shift+S**) |
| Sobra o falta una llave `}` | Presiona **Ctrl+K, Ctrl+D** para acomodar el código. Al poner el cursor junto a una llave, VS te muestra su pareja |

---

## Antes de entregar

- [ ] Las clases de las tablas son `public`, tienen `Id` y sus llaves foráneas
- [ ] No quedó ningún nombre del ejercicio que copiaste (`tienda`, `Ventas`, `Clientes`…)
- [ ] `IConexion` y `Conexion` tienen las mismas tablas con los mismos nombres
- [ ] Cada interfaz tiene su clase, y lo extra está en las dos
- [ ] Si el profe pidió reglas, están todas (al crear **y** al editar)
- [ ] La base existe y en SQL Server se ven las tablas con sus llaves
- [ ] `servicios_X` es el proyecto de inicio y `Program.cs` tiene la línea del ciclo
- [ ] Hay un controller por tabla, más lo extra que pidan
- [ ] Probaste listar, crear, editar y borrar, y la consulta que une las tablas trae los datos completos
- [ ] Compila con 0 errores
