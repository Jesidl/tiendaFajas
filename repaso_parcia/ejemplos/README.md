# Ejemplos de repaso: migración EF Core + servicio web (ejemplos 3 a 7)

Son 5 ejercicios completos, con la misma arquitectura del profe (`libreria_X` + `servicios_X`).
La estructura es **siempre la misma**. Lo que cambia entre un ejercicio y otro es la **lógica dentro de las clases `...Aplicacion`**, la parte que en `examen.txt` aparece como `//conexiones//`.
Por eso este README se centra en eso.

| # | Carpeta | Tema | Relación entre tablas | Lo nuevo que enseña |
|---|---|---|---|---|
| 3 | `ejemplo3_clinica` | Citas médicas | Detalle con 2 FK (como tienda) | Validar contra la BD, choque de horario, edad, descuentos encadenados, borrado lógico, estados |
| 4 | `ejemplo4_biblioteca` | Préstamos | Detalle con 2 FK | **Modificar otra tabla** (stock), fechas límite, multas, `DateTime?`, constantes, `IQueryable` reutilizable |
| 5 | `ejemplo5_banco` | Cuentas | **En cadena** A→B→C | `ThenInclude`, Include de lista, saldo acumulado, campos protegidos, código consecutivo, insertar automáticamente en otra tabla, operación prohibida, DTO de extracto |
| 6 | `ejemplo6_universidad` | Matrículas | **Muchos a muchos** con datos | Cupos con `Count`, límite con `Sum` sobre otra tabla, duplicados de 3 campos, `switch`, estado derivado, reportes `GroupBy` y promedio ponderado |
| 7 | `ejemplo7_parqueadero` | Parqueadero | Detalle con 2 FK | Tiempo con `TimeSpan`, tarifa por hora o fracción, división entera y residuo, tope, asignación automática, liberar uno y ocupar otro, normalizar texto |

---

## Cómo abrir y probar cada ejemplo

1. Abre `ejemploN_xxx/xxx.slnx` en Visual Studio.
2. La **migración ya está creada** (carpeta `libreria_xxx/Migrations`) y la base **ya existe** en `localhost\SQLEXPRESS`.
   - Si la borras o la quieres recrear: elige la librería como proyecto de inicio y ejecuta `Update-Database` en la Consola del Administrador de paquetes.
   - Para practicar la migración desde cero: borra la carpeta `Migrations` y ejecuta `Add-Migration migration1` y luego `Update-Database`.
3. Elige `servicios_xxx` como proyecto de inicio y presiona **Ctrl+F5**. Con F5, Visual Studio se detiene en cada `throw`.
4. Abre `servicios_xxx.http` y haz clic en **Send request** en orden. Empieza siempre por el `Parcial`.

| Ejemplo | Puerto | Base de datos |
|---|---|---|
| 3 Clínica | 5203 | db_clinica |
| 4 Biblioteca | 5204 | db_biblioteca |
| 5 Banco | 5205 | db_banco |
| 6 Universidad | 5206 | db_universidad |
| 7 Parqueadero | 5207 | db_parqueadero |

**Cosas normales al probar:**
- Un error de negocio responde **500** con el texto `System.Exception: <tu mensaje>`. Eso es el `throw` funcionando.
- En las respuestas de Insertar o `Parcial` puedes ver `"citas":[null]`. Es `IgnoreCycles` cortando el ciclo: la lista contenía la misma cita que se está devolviendo. No es un error.
- Cada ejemplo tiene un `Script.sql` comentado con el SQL equivalente, por si te piden el script a mano.

---

## Ejemplo 3: Clínica (citas médicas)

### Enunciado
Una clínica necesita agendar citas de sus pacientes con sus médicos.

**Tablas**
- **Pacientes:** Id, Documento, Nombre, FechaNacimiento, Subsidiado (bool), Activo (bool)
- **Medicos:** Id, Nombre, Especialidad, Tarifa (decimal), Activo (bool)
- **Citas:** Id, Paciente (FK), Medico (FK), Fecha (fecha y hora), Estado, Valor, Observacion

**Reglas**
1. El documento del paciente es obligatorio y **único**.
2. La tarifa del médico debe ser mayor a 0.
3. Toda cita nueva nace en estado **"Programada"**; el usuario no elige el estado.
4. La cita debe ser en una **fecha futura**.
5. El paciente y el médico deben existir y estar **activos**.
6. Un médico **no puede tener dos citas a menos de 30 minutos** de diferencia (las canceladas no cuentan).
7. Un paciente solo puede tener **una cita por día**.
8. **Valor** = tarifa del médico, con 50% de descuento si el paciente es subsidiado y luego 20% adicional si tiene 60 años o más.
9. Al modificar una cita solo se reprograma la fecha o el médico; el estado no se puede cambiar por PUT. Solo se modifican las citas programadas.
10. **Borrar una cita = cancelarla** (Estado "Cancelada"). No se puede cancelar una cita atendida.
11. **Atender** una cita cambia su estado a "Atendida" y guarda la observación.
12. Borrar un paciente = **desactivarlo**, y no se permite si tiene citas programadas.
13. Un médico solo se puede borrar si **no tiene ninguna cita**.
14. Listados: pacientes activos ordenados por nombre; médicos activos ordenados por especialidad y nombre; citas con paciente y médico, ordenadas por fecha; citas de un médico.

**Endpoints:** `GET /Citas`, `GET /Citas/PorMedico/{medico}`, `GET /Citas/Parcial`, `PUT /Citas/Atender/{id}?observacion=...` y el CRUD de las 3 tablas.

**Comprobación:** `/Citas/Parcial` (paciente de 1955, subsidiado, tarifa 100000) debe dar **valor 40000**.

---

## Ejemplo 4: Biblioteca (préstamos)

### Enunciado
**Tablas**
- **Libros:** Id, Isbn, Titulo, Autor, Ejemplares, Disponibles
- **Lectores:** Id, Documento, Nombre, Correo, Activo
- **Prestamos:** Id, Libro (FK), Lector (FK), FechaPrestamo, FechaLimite, FechaDevolucion (**puede ser nula**), DiasRetraso, Multa, Devuelto

**Reglas**
1. Al crear un libro, `Disponibles = Ejemplares`. El usuario no lo envía.
2. Si se cambia el número de ejemplares, los disponibles se recalculan. No pueden quedar menos ejemplares que los que están prestados.
3. El correo del lector debe tener `@` y `.`, y el documento es único.
4. Al **prestar**:
   - La fecha de préstamo es hoy y la **fecha límite es +15 días**.
   - El libro debe tener disponibles, y se **descuenta 1** al libro.
   - El lector debe estar activo, con **máximo 3 préstamos sin devolver**.
   - No se presta si el lector tiene **préstamos vencidos**, ni si ya tiene prestado ese mismo libro.
5. Al **devolver** (`PUT /Prestamos/Devolver/{id}`):
   - Se guarda la fecha de devolución.
   - Se calculan los días de retraso (mínimo 0) y la **multa = días × 1000**.
   - El libro **recupera 1** disponible.
6. **Renovar** (PUT): solo se cambia la fecha límite, que debe ser posterior a la actual y no más de 30 días después del préstamo.
7. Borrar un préstamo sin devolver devuelve el ejemplar al stock.
8. No se borra un libro con préstamos pendientes. Un lector con libros sin devolver no se puede desactivar.
9. Consultas: libros disponibles, préstamos pendientes, préstamos vencidos, préstamos por lector.

**Comprobación:** `/Prestamos/Parcial` crea un préstamo de hace 20 días. Al devolverlo debe dar **5 días de retraso y multa de 5000**, y los disponibles del libro vuelven a 3.

---

## Ejemplo 5: Banco (relación en cadena)

### Enunciado
**Tablas:** Clientes 1→N Cuentas 1→N Movimientos
- **Clientes:** Id, Documento, Nombre, Telefono, Activo
- **Cuentas:** Id, Numero, Cliente (FK), Tipo, Saldo, FechaApertura, Activa
- **Movimientos:** Id, Cuenta (FK), Tipo, Valor, Impuesto, SaldoAnterior, SaldoNuevo, Fecha, Descripcion

**Reglas**
1. El tipo de cuenta solo puede ser "Ahorros" o "Corriente". Una cuenta de ahorros se abre con **mínimo 50000**.
2. Un cliente tiene como **máximo 3 cuentas activas**.
3. El **número de cuenta lo genera el sistema**: `AH-000001` o `CO-000002`.
4. Si la cuenta se abre con saldo, se registra **automáticamente** un movimiento de "Apertura".
5. Por PUT **no se puede cambiar** el saldo, el número, el cliente, el tipo ni la fecha de apertura. Solo se puede bloquear o desbloquear la cuenta (`activa`).
6. Una cuenta solo se borra si tiene **saldo 0**.
7. Movimiento "Deposito": suma al saldo. Movimiento "Retiro":
   - Cobra el **4x1000**.
   - No puede dejar el saldo negativo.
   - El límite es de **2.000.000 retirados por día**.
8. Cada movimiento guarda el saldo anterior y el saldo nuevo, y **actualiza el saldo de la cuenta**.
9. Los movimientos **no se pueden modificar ni borrar**.
10. No se puede mover dinero en una cuenta bloqueada.
11. Reportes:
    - Clientes con sus cuentas.
    - Movimientos con su cuenta y el cliente de la cuenta.
    - **Extracto** de una cuenta: totales de depósitos, retiros e impuestos, y la lista de movimientos.

**Comprobación:**
- `/Cuentas/Parcial` crea la cuenta `AH-000001` con 500000 y el movimiento de Apertura.
- `/Movimientos/Parcial/1` hace un retiro de 100000 con impuesto de 400, y el saldo queda en **399600**.

---

## Ejemplo 6: Universidad (muchos a muchos)

### Enunciado
**Tablas**
- **Estudiantes:** Id, Codigo, Nombre, Estrato (1–6), Activo
- **Cursos:** Id, Codigo, Nombre, Creditos (1–5), Cupo, ValorCredito
- **Matriculas:** Id, Estudiante (FK), Curso (FK), Periodo, Fecha, Valor, Descuento, Total, Nota (**puede ser nula**), Estado

**Reglas**
1. El estrato va de 1 a 6, los créditos de 1 a 5, y el cupo y el valor del crédito son mayores a 0.
2. No se repite la combinación **estudiante + curso + periodo** (sin contar las canceladas).
3. Se respeta el **cupo** del curso en el periodo.
4. Un estudiante no puede pasar de **20 créditos por periodo**.
5. **Valor** = créditos × valor del crédito. **Descuento** según estrato: 1–2 → 50%, 3 → 25%, 4–6 → 0%. **Total** = valor − descuento.
6. **Calificar** (`PUT /Matriculas/Calificar/{id}/{nota}`): la nota va de 0 a 5. Con nota ≥ 3.0 queda "Aprobada"; si no, "Reprobada". Solo se califican las matrículas activas.
7. PUT = **cambiar de curso**. Se vuelven a validar el cupo, el duplicado y los créditos (sin contarse a sí misma), y se recalculan los valores.
8. Borrar = **cancelar**, y solo si la matrícula sigue activa.
9. No se puede bajar el cupo de un curso por debajo de sus inscritos.
10. Reportes:
    - **Resumen por curso** en un periodo: inscritos, cupos libres, recaudo y promedio.
    - **Promedio ponderado** de un estudiante.

**Comprobación:** `/Matriculas/Parcial` matricula a un estudiante de estrato 2 en un curso de 3 créditos × 200000: **valor 600000, descuento 300000, total 300000**.

---

## Ejemplo 7: Parqueadero (tiempo y tarifas)

### Enunciado
**Tablas**
- **Vehiculos:** Id, Placa, Tipo (Carro/Moto), Propietario, Mensualidad
- **Celdas:** Id, Codigo, Tipo (Carro/Moto), Ocupada
- **Registros:** Id, Vehiculo (FK), Celda (FK), Entrada, Salida (**puede ser nula**), Minutos, Valor, Cerrado

**Reglas**
1. La placa se guarda **sin espacios ni guiones y en mayúscula**, tiene 6 caracteres y es única.
2. **Entrada** (POST Registros):
   - El vehículo no puede estar ya adentro.
   - Si `celda = 0`, se **asigna automáticamente** la primera libre de su tipo.
   - Si se indica la celda, debe estar libre y ser del mismo tipo que el vehículo.
   - La celda queda **ocupada**.
3. **Salida** (`PUT /Registros/Salida/{id}`):
   - Calcula los minutos, redondeados hacia arriba.
   - Calcula el valor y **libera la celda**.
4. **Tarifa:**
   - Mensualidad → 0.
   - Los primeros 15 minutos son gratis.
   - Por **hora o fracción**: carro 4000, moto 2000.
   - Cada día completo cobra el **tope** (carro 30000, moto 15000), y el tiempo restante nunca supera el tope.
5. PUT = **mover a otra celda**: libera la anterior y ocupa la nueva, que debe estar libre y ser del mismo tipo.
6. El campo `Ocupada` de una celda no se edita por PUT.
7. Reportes:
   - Vehículos adentro.
   - Celdas disponibles por tipo.
   - **Recaudo del día** agrupado por tipo de vehículo.

**Comprobación:**
- `/Registros/Parcial` registra un carro que entró hace 2 h 10 min. Al darle salida: **131 minutos = 3 horas = 12000**.
- Un carro con 2084 minutos (1 día + 644 minutos) paga **30000 + 30000 = 60000**.

---

# Catálogo de patrones de la "conexión" (lógica en `...Aplicacion`)

Esta es la parte que cambia según lo que pida el profe. Lee el enunciado, busca la frase en la tabla y copia el patrón del archivo indicado.

## A. Traducir el enunciado a un patrón

| Si el enunciado dice… | Patrón | Ejemplo de dónde copiarlo |
|---|---|---|
| "obligatorio" | A1 Campo obligatorio | Clínica `PacientesAplicacion.Insertar` |
| "único", "no se puede repetir" | A2 Único (`Any`) | Clínica `PacientesAplicacion` |
| "no se puede repetir la combinación…" | A3 Duplicado de varios campos | Universidad `ValidarReglasDeCupo` |
| "entre X y Y", "mayor a 0" | A4 Rango | Universidad `ValidarEstrato` |
| "solo puede ser A o B" | A5 Lista de valores | Banco `CuentasAplicacion.Insertar` |
| "debe existir", "debe estar activo" | A6 Existe y activo | Clínica `CitasAplicacion.Insertar` |
| "máximo N por…" | B1 Límite con `Count` | Biblioteca (3 préstamos), Banco (3 cuentas) |
| "cupo" | B1 Límite con `Count` | Universidad |
| "no puede superar X en total…" | B2 Límite acumulado con `Sum` | Banco (retiro diario), Universidad (créditos) |
| "no puede cruzarse", "mismo horario" | B3 Choque de horario | Clínica `ValidarChoqueHorario` |
| "el mismo día" | B4 Rango de un día | Clínica, Banco |
| "calcular", "total", "valor" | C1 Cálculo | Todos |
| "descuento según…" | C2 `switch` / `if` encadenados | Universidad `CalcularValores`, Clínica `CalcularValor` |
| "impuesto X%" | C3 Porcentaje + `Math.Round` | Banco (4x1000) |
| "edad" | C4 Edad | Clínica `CalcularEdad` |
| "días de retraso", "multa" | C5 Diferencia de fechas | Biblioteca `Devolver` |
| "por hora o fracción", "minutos" | C6 `TimeSpan` + `Math.Ceiling` | Parqueadero `Salida` |
| "tope", "máximo a cobrar" | C7 `Math.Min` | Parqueadero `CalcularValor` |
| "promedio ponderado" | C8 Suma de productos / suma de pesos | Universidad `Promedio` |
| "al registrar X se descuenta / suma en Y" | D1 Modificar otra tabla | Biblioteca (stock), Banco (saldo) |
| "se registra automáticamente…" | D2 Insertar en otra tabla | Banco (movimiento de Apertura) |
| "mover", "trasladar" | D3 Liberar uno y ocupar otro | Parqueadero `Actualizar` |
| "lo genera el sistema", "consecutivo" | E1 Código generado | Banco `GenerarNumero` |
| "nace con estado…", "por defecto" | E2 Valor por defecto | Clínica (Programada), todos (`Activo = true`) |
| "si no se indica, se asigna…" | E3 Asignación automática | Parqueadero (celda 0) |
| "se guarda en mayúscula / sin espacios" | E4 Normalizar texto | Parqueadero `Normalizar` |
| "no se puede modificar el campo X" | F1 Campos protegidos | Banco `CuentasAplicacion.Actualizar` |
| "solo se puede cambiar X" | F2 Solo un campo editable | Biblioteca `Actualizar` (renovar) |
| "solo si está en estado X" | F3 Regla de estado | Clínica, Universidad |
| "cambia a Aprobado si…" | F4 Estado derivado (ternario) | Universidad `Calificar` |
| "no se elimina, se desactiva / cancela" | G1 Borrado lógico | Clínica (paciente y cita), Universidad |
| "no se puede borrar si tiene…" | G2 Borrado con restricción | Clínica `MedicosAplicacion.Borrar` |
| "solo si el saldo / valor es 0" | G3 Condición de valor para borrar | Banco `CuentasAplicacion.Borrar` |
| "no se pueden borrar ni modificar" | G4 Operación prohibida | Banco `MovimientosAplicacion` |
| "listar solo los activos / ordenados" | H1 `Where` + `OrderBy` / `ThenBy` | Todos |
| "consultar por X" | H2 Consulta con parámetro | Clínica `ConsultarPorMedico` |
| "mostrar con sus datos relacionados" | H3 `Include` + limpiar ciclos | Todos |
| "…y el cliente de la cuenta" | H4 `ThenInclude` | Banco `MovimientosAplicacion.Consultar` |
| "el cliente con sus cuentas" | H5 Include de una lista | Banco `ClientesAplicacion.Consultar` |
| "pendientes", "vencidos" | H6 Filtros con fecha o estado | Biblioteca |
| "reporte", "resumen", "total por…" | I1 DTO + `GroupBy` | Universidad, Parqueadero, Banco (Extracto) |

## B. Los patrones con código

### A. Validaciones (siempre al inicio, con `throw`)
```csharp
// A1 obligatorio
if (string.IsNullOrWhiteSpace(entidad.Documento))
    throw new Exception("El documento es obligatorio");

// A2 unico (al insertar)
if (this.conexion.Pacientes!.Any(x => x.Documento == entidad.Documento))
    throw new Exception("Ya existe");
// A2 unico (al actualizar) -> excluirse a si mismo
if (this.conexion.Pacientes!.Any(x => x.Documento == entidad.Documento && x.Id != entidad.Id))
    throw new Exception("Ya existe otro");

// A3 duplicado de varios campos
if (this.conexion.Matriculas!.Any(x => x.Estudiante == entidad.Estudiante
        && x.Curso == entidad.Curso && x.Periodo == entidad.Periodo && x.Estado != "Cancelada"))
    throw new Exception("Ya matriculado");

// A4 rango
if (entidad.Estrato < 1 || entidad.Estrato > 6) throw new Exception("...");

// A5 lista de valores
if (entidad.Tipo != "Ahorros" && entidad.Tipo != "Corriente") throw new Exception("...");

// A6 existe y activo
var paciente = this.conexion.Pacientes!.FirstOrDefault(x => x.Id == entidad.Paciente);
if (paciente == null || !paciente.Activo) throw new Exception("...");
```

### B. Límites contra la base de datos
```csharp
// B1 limite con Count
int pendientes = this.conexion.Prestamos!.Count(x => x.Lector == entidad.Lector && !x.Devuelto);
if (pendientes >= 3) throw new Exception("Maximo 3");

// B2 limite acumulado con Sum (puede sumar un campo de OTRA tabla: EF hace el JOIN)
int creditos = this.conexion.Matriculas!
    .Where(x => x.Estudiante == entidad.Estudiante && x.Periodo == entidad.Periodo)
    .Sum(x => x._Curso!.Creditos);
if (creditos + curso.Creditos > 20) throw new Exception("...");

// B3 choque de horario (variables ANTES de la consulta)
var desde = entidad.Fecha.AddMinutes(-30);
var hasta = entidad.Fecha.AddMinutes(30);
bool choca = this.conexion.Citas!.Any(x => x.Medico == entidad.Medico
    && x.Id != entidad.Id && x.Fecha > desde && x.Fecha < hasta);

// B4 "el mismo dia"
var inicio = entidad.Fecha.Date;          // 00:00
var fin = inicio.AddDays(1);              // 00:00 del dia siguiente
... x.Fecha >= inicio && x.Fecha < fin
```

### C. Cálculos
```csharp
// C2 descuento por categoria
decimal porcentaje;
switch (estudiante.Estrato)
{
    case 1: case 2: porcentaje = 0.50m; break;
    case 3: porcentaje = 0.25m; break;
    default: porcentaje = 0m; break;
}

// C3 porcentaje + redondeo  (la "m" = numero decimal)
entidad.Impuesto = Math.Round(entidad.Valor * 0.004m, 2);

// C4 edad
int edad = DateTime.Today.Year - nacimiento.Year;
if (nacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

// C5 diferencia de dias
int dias = (devolucion.Date - limite.Date).Days;
int retraso = Math.Max(0, dias);          // nunca negativo

// C6 minutos y horas o fraccion
TimeSpan tiempo = salida - entrada;
int minutos = (int)Math.Ceiling(tiempo.TotalMinutes);
int horas = (int)Math.Ceiling(minutos / 60.0);   // 60.0 -> division con decimales

// C7 tope
decimal valor = Math.Min(horas * tarifa, tope);

// division entera y residuo
int diasCompletos = minutos / 1440;   // int / int = entero
int sobrante = minutos % 1440;        // residuo

// C8 promedio ponderado (evitar division por 0)
int pesos = lista.Sum(x => x._Curso!.Creditos);
if (pesos > 0) promedio = lista.Sum(x => x.Nota!.Value * x._Curso!.Creditos) / pesos;
```

### D. Afectar otras tablas (el más importante)
```csharp
// D1 MODIFICAR OTRA TABLA: consultar -> cambiar -> marcar Modified -> UN SaveChanges
var libro = this.conexion.Libros!.FirstOrDefault(x => x.Id == entidad.Libro);
if (libro == null) throw new Exception("...");
if (libro.Disponibles <= 0) throw new Exception("Sin stock");
libro.Disponibles = libro.Disponibles - 1;
this.conexion.Entry<Libros>(libro).State = EntityState.Modified;   // NoTracking -> obligatorio
this.conexion.Prestamos!.Add(entidad);
this.conexion.SaveChanges();                                       // todo o nada

// D2 INSERTAR EN OTRA TABLA usando la navegacion (aun no hay Id)
var mov = new Movimientos();
mov._Cuenta = entidad;          // EF inserta la cuenta, toma su Id y lo pone en mov.Cuenta
mov.Tipo = "Apertura";
this.conexion.Movimientos!.Add(mov);
this.conexion.Cuentas!.Add(entidad);
this.conexion.SaveChanges();

// D3 LIBERAR UNO Y OCUPAR OTRO
celdaVieja.Ocupada = false;  this.conexion.Entry<Celdas>(celdaVieja).State = EntityState.Modified;
celdaNueva.Ocupada = true;   this.conexion.Entry<Celdas>(celdaNueva).State = EntityState.Modified;
original.Celda = celdaNueva.Id; this.conexion.Entry<Registros>(original).State = EntityState.Modified;
this.conexion.SaveChanges();
```
**Por qué `Entry(...).State = Modified`:** la `Conexion` tiene `NoTracking`, así que EF **no** detecta solo que cambiaste un objeto consultado. Si te falta esta línea, el cambio **no se guarda** y tampoco sale ningún error.

### E. Valores que pone el sistema
```csharp
entidad.Estado = "Programada";                         // E2 por defecto
entidad.Activo = true;
if (entidad.Fecha == default(DateTime))                // "no la mandaron"
    entidad.Fecha = DateTime.Now;
entidad.FechaLimite = entidad.FechaPrestamo.AddDays(15);

string numero = "AH-" + consecutivo.ToString("D6");    // E1 -> AH-000001

entidad.Placa = (entidad.Placa ?? "").Replace(" ", "").ToUpper();   // E4 normalizar
```

### F. Actualizar con reglas: **cargar primero el original**
```csharp
var original = this.conexion.Cuentas!.FirstOrDefault(x => x.Id == entidad.Id);
if (original == null) throw new Exception("No existe");

// F3 regla de estado
if (original.Estado != "Programada") throw new Exception("Solo programadas");

// F1 campos protegidos: se copian del original
entidad.Saldo = original.Saldo;
entidad.Numero = original.Numero;
// ... y se guarda "entidad"

// F2 solo un campo editable: se cambia en el original y se guarda "original"
original.FechaLimite = entidad.FechaLimite;
this.conexion.Entry<Prestamos>(original).State = EntityState.Modified;

// F4 estado derivado
matricula.Estado = nota >= 3.0m ? "Aprobada" : "Reprobada";
```

### G. Borrar con reglas
```csharp
// G1 BORRADO LOGICO (del JSON solo importa el Id)
var cita = this.conexion.Citas!.FirstOrDefault(x => x.Id == entidad.Id);
cita.Estado = "Cancelada";                   // o: paciente.Activo = false;
this.conexion.Entry<Citas>(cita).State = EntityState.Modified;
this.conexion.SaveChanges();

// G2 no borrar si tiene hijos
if (this.conexion.Citas!.Any(x => x.Medico == entidad.Id))
    throw new Exception("Tiene citas");
this.conexion.Medicos!.Remove(entidad);

// G3 condicion de valor
if (cuenta.Saldo != 0) throw new Exception("Debe tener saldo 0");

// G4 prohibido
public Movimientos Borrar(Movimientos entidad)
{
    throw new Exception("Los movimientos no se pueden borrar");
}
```
**Ojo con el borrado físico:** la FK tiene `onDelete: Cascade`. Si borras un padre, **se borran todos sus hijos**. Por eso el borrado lógico es tan común.

### H. Consultas
```csharp
// H1 filtrar y ordenar
.Where(x => x.Activo).OrderBy(x => x.Especialidad).ThenBy(x => x.Nombre)
.OrderByDescending(x => x.Fecha)                          // mas recientes primero
.ThenBy(x => x._Estudiante!.Nombre)                       // ordenar por un campo del Include

// H3 Include desde el hijo + limpiar ciclo
var lista = this.conexion.Citas!.Include(x => x._Paciente).Include(x => x._Medico).ToList();
foreach (var item in lista) { item._Paciente!.Citas = null; item._Medico!.Citas = null; }

// H4 cadena: ThenInclude
.Include(x => x._Cuenta).ThenInclude(c => c!._Cliente)
// limpiar: item._Cuenta!.Movimientos = null; item._Cuenta._Cliente!.Cuentas = null;

// H5 Include de una lista (desde el padre)
.Include(x => x.Cuentas)
// limpiar: foreach cuenta in cliente.Cuentas -> cuenta._Cliente = null;

// consulta base reutilizable (IQueryable todavia no va a la BD)
private IQueryable<Prestamos> ConsultaBase()
{
    return this.conexion.Prestamos!.Include(x => x._Libro).Include(x => x._Lector);
}
// ConsultaBase().Where(x => !x.Devuelto).ToList();
```

### I. Reportes (DTO, no se guardan)
1. Clase en `entidades` **sin DbSet**, así no se vuelve tabla.
2. Firma en la **interfaz**.
3. Método en la Aplicación:
```csharp
var datos = this.conexion.Matriculas!.Include(x => x._Curso)
    .Where(x => x.Periodo == periodo).ToList();     // primero a memoria
foreach (var grupo in datos.GroupBy(x => x.Curso))  // grupo.Key = valor agrupado
{
    var r = new ResumenCurso();
    r.Curso = grupo.First()._Curso!.Nombre;
    r.Inscritos = grupo.Count();
    r.Recaudo = grupo.Sum(x => x.Total);
    var conNota = grupo.Where(x => x.Nota != null).ToList();
    r.Promedio = conNota.Count > 0 ? conNota.Average(x => x.Nota!.Value) : null;  // Average con lista vacia falla
    resultado.Add(r);
}
```
4. Endpoint en el controller con su **propia ruta**: `[HttpGet("Resumen/{periodo}")]`.

## C. Rutas en el controller

| Necesitas | Atributo | URL |
|---|---|---|
| Listar | `[HttpGet]` | `/Citas` |
| Otro listado | `[HttpGet("Pendientes")]` | `/Prestamos/Pendientes` |
| Con parámetro | `[HttpGet("PorMedico/{medico}")]` + `int medico` | `/Citas/PorMedico/1` |
| Parámetro de texto | `[HttpGet("Disponibles/{tipo}")]` + `string tipo` | `/Celdas/Disponibles/Carro` |
| Acción sobre un registro | `[HttpPut("Devolver/{id}")]` + `int id` | `PUT /Prestamos/Devolver/1` |
| Dos parámetros | `[HttpPut("Calificar/{id}/{nota}")]` | `PUT /Matriculas/Calificar/1/4.5` |
| Opcional (query) | `[HttpGet("Recaudo")]` + `DateTime? fecha` | `/Registros/Recaudo?fecha=2026-10-07` |
| Texto extra (query) | `[HttpPut("Atender/{id}")]` + `string? observacion` | `/Citas/Atender/1?observacion=ok` |

El nombre entre `{ }` debe ser **igual** al nombre del parámetro del método.

## D. Tipos que pueden ser nulos

| C# | SQL | Cuándo usarlo |
|---|---|---|
| `DateTime?` | `datetime2 NULL` | Fecha que todavía no existe (devolución, salida) |
| `decimal?` | `decimal NULL` | Valor que aún no se conoce (nota) |
| `int?` | `int NULL` | **FK opcional**: el hijo puede no tener padre; sin Cascade obligatorio |

Para leer el valor: `x.FechaDevolucion.Value`, o preguntar primero `x.Nota != null`.

---

## Orden de estudio sugerido
1. **Clínica:** validaciones y estados. Es la base de todo.
2. **Biblioteca:** modificar otra tabla. Es lo que más piden.
3. **Universidad:** `Count`, `Sum` y reportes.
4. **Banco:** relación en cadena y campos protegidos.
5. **Parqueadero:** matemáticas con tiempo.

Para practicar: copia la estructura de **tienda** y, sin mirar, implementa las reglas de uno de estos enunciados. Después compara tu versión con la carpeta del ejemplo.
