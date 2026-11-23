using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.nucleo;
using pruebas_tiendaFajas.Nucleo;
using System.Text;

namespace pruebas_tiendaFajas.Comunicaciones
{
    /// <summary>
    /// Prueba de integracion del transporte real.
    ///
    /// Las pruebas de ContratoPresentacionPrueba usan un doble y por eso no
    /// tocan la red: comprueban la logica del front. Esta hace lo contrario,
    /// usa el Comunicaciones de verdad y recorre el camino completo
    ///
    ///     front -> HTTP -> servicio -> Entity Framework -> SQL Server
    ///
    /// que es lo unico que puede demostrar que las dos mitades hablan el mismo
    /// idioma: que la URL existe, que el JSON que manda una lo entiende la
    /// otra, que la cadena de Replace deja la respuesta deserializable, y que
    /// la cabecera X-Usuario llega hasta la auditoria.
    ///
    /// Necesita el servicio levantado. Cuando no lo esta se declara
    /// Inconclusive y no fallida: una prueba de integracion que falla porque
    /// no se levanto una dependencia no esta informando de ningun defecto, y
    /// marcarla en rojo solo enseña a ignorar el rojo.
    /// </summary>
    [TestClass]
    public class ServicioEnVivoPrueba
    {
        // la misma direccion que arma Comunicaciones. Esta repetida a
        // proposito: si alguien cambia el puerto alla y no aca, esta prueba se
        // declara Inconclusive y deja ver que las dos quedaron desalineadas.
        private const string Servicio = "http://localhost:5250/";

        private const string Etiqueta = "prueba:ServicioEnVivo";

        private static bool Arriba()
        {
            try
            {
                using var cliente = new HttpClient();
                cliente.Timeout = TimeSpan.FromSeconds(5);

                var cuerpo = new StringContent("{}", Encoding.UTF8, "application/json");
                var respuesta = cliente
                    .PostAsync(Servicio + "Cargos/Consultar", cuerpo)
                    .Result;

                return respuesta.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        [TestMethod]
        public void ElCicloCompletoViajaPorHttp()
        {
            if (!Arriba())
            {
                Assert.Inconclusive(
                    "el servicio no responde en " + Servicio + ": levantelo con " +
                    "\"dotnet run --project backend/servicio_tiendaFajas\" para " +
                    "correr esta prueba");
                return;
            }

            // el usuario de la sesion es lo que Comunicaciones pone en la
            // cabecera X-Usuario, y lo que el servicio escribe en la auditoria
            SesionActual.Usuario = Etiqueta;

            var transporte = new presentacion_tiendaFajas.implementaciones.Comunicaciones();
            var presentacion = new aplicacion_tiendaFajas.implementaciones
                .CargosPresentacion(transporte);

            Cargos? insertado = null;
            IConexion conexion = new Conexion();

            try
            {
                // ---------- insertar ----------
                insertado = presentacion.Insertar(new Cargos
                {
                    Nombre = EntidadesNucleo.Unico("Cargo ", 100),
                    Descripcion = "insertado por HTTP",
                    Salario = 1450000.00m
                });

                Assert.IsNotNull(insertado, "el servicio no devolvio la entidad");
                Assert.IsGreaterThan(0, insertado!.Id,
                    "el servicio devolvio la entidad sin Id: el Id se asigna en " +
                    "la base y tiene que volver por la respuesta");

                // ---------- llego de verdad a SQL Server ----------
                var llave = insertado.Id;
                var enBase = conexion.Cargos!.FirstOrDefault(x => x.Id == llave);
                Assert.IsNotNull(enBase,
                    "el servicio respondio bien pero la fila no esta en la base");
                Assert.AreEqual("insertado por HTTP", enBase!.Descripcion,
                    "la descripcion no llego igual hasta la base");

                // ---------- buscar ----------
                var encontrados = presentacion.Buscar(
                    new Cargos { Nombre = insertado.Nombre });

                Assert.IsTrue(encontrados.Any(x => x.Id == llave),
                    "Buscar no devolvio la fila que se acaba de insertar");

                // ---------- actualizar ----------
                insertado.Descripcion = "actualizado por HTTP";
                presentacion.Actualizar(insertado);

                var actualizado = conexion.Cargos!.FirstOrDefault(x => x.Id == llave);
                Assert.AreEqual("actualizado por HTTP", actualizado!.Descripcion,
                    "la actualizacion no llego a la base");

                // ---------- la cabecera X-Usuario llego a la auditoria ----------
                var auditado = conexion.Auditorias!.Any(x =>
                    x.Entidad == "Cargos" &&
                    x.Llave == llave &&
                    x.Usuario == Etiqueta);

                Assert.IsTrue(auditado,
                    "la auditoria no registro el usuario de la sesion: la cabecera " +
                    "X-Usuario no esta llegando desde el front hasta el servicio");

                // ---------- borrar ----------
                presentacion.Borrar(insertado);
                insertado = null;

                Assert.IsNull(conexion.Cargos!.FirstOrDefault(x => x.Id == llave),
                    "la fila sigue en la base despues de borrarla por HTTP");
            }
            finally
            {
                SesionActual.Usuario = null;
                Limpiar(conexion, insertado);
            }
        }

        /// <summary>
        /// Deja la base como estaba: borra la fila si la prueba fallo antes de
        /// llegar a borrarla, y se lleva los rastros de auditoria propios.
        /// </summary>
        private static void Limpiar(IConexion conexion, Cargos? pendiente)
        {
            if (pendiente != null)
            {
                try
                {
                    conexion.Cargos!.Remove(pendiente);
                    conexion.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[" + Etiqueta +
                        "] no se pudo borrar la fila de prueba: " + ex.Message);
                }
            }

            try
            {
                var rastros = conexion.Auditorias!
                    .Where(x => x.Usuario == Etiqueta)
                    .ToList();

                if (rastros.Count == 0)
                    return;

                conexion.Auditorias!.RemoveRange(rastros);
                conexion.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + Etiqueta +
                    "] no se pudo limpiar la auditoria: " + ex.Message);
            }
        }
    }
}
