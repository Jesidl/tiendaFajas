using Newtonsoft.Json;
using presentacion_tiendaFajas.interfaces;

namespace pruebas_tiendaFajas.Nucleo
{
    /// <summary>
    /// Doble de prueba de IComunicaciones.
    ///
    /// La capa de presentacion (CargosPresentacion y sus veintitres hermanas)
    /// recibe IComunicaciones por el constructor, asi que se puede probar sin
    /// servicio, sin base de datos y sin red: esto reemplaza el transporte y
    /// deja ver con que URL y con que entidad se llamo.
    ///
    /// Sin esto, probar la capa de presentacion exigiria levantar el servicio,
    /// y una prueba que necesita un proceso aparte corriendo no es una prueba
    /// unitaria sino de integracion. Las dos estan: esta es la unitaria.
    /// </summary>
    public class ComunicacionesFalsas : IComunicaciones
    {
        /// <summary>URL con la que se llamo la ultima vez.</summary>
        public string? Url { get; private set; }

        /// <summary>Entidad que se envio la ultima vez.</summary>
        public object? Enviado { get; private set; }

        /// <summary>Cuantas veces se llamo en total.</summary>
        public int Llamadas { get; private set; }

        /// <summary>
        /// Lo que debe devolver. Si queda en null se devuelve de vuelta la
        /// misma entidad que se envio, que es lo que hace el servicio real en
        /// Insertar, Actualizar y Borrar.
        /// </summary>
        public object? Respuesta { get; set; }

        /// <summary>
        /// Si se llena, Ejecutar lanza esta excepcion. Sirve para comprobar
        /// que la capa de presentacion no se traga los errores del transporte.
        /// </summary>
        public Exception? Falla { get; set; }

        public Task<T> Ejecutar<T>(string url, object entidad)
        {
            this.Llamadas++;
            this.Url = url;
            this.Enviado = entidad;

            // se devuelve una tarea fallida y no se lanza aqui mismo, porque
            // el Comunicaciones real es async: el error llega al llamador
            // envuelto en AggregateException al hacer task.Wait(). Lanzar de
            // forma sincrona cambiaria ese comportamiento y la prueba dejaria
            // de parecerse a la realidad.
            if (this.Falla != null)
                return Task.FromException<T>(this.Falla);

            if (this.Respuesta != null)
                return Task.FromResult((T)this.Respuesta);

            // Consultar y Buscar esperan List<T>, y la entidad que se envio es
            // solo el filtro: no se puede convertir una en la otra. Se devuelve
            // una lista vacia, y la prueba que necesite filas llena Respuesta.
            if (typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                return Task.FromResult(Activator.CreateInstance<T>());

            // Insertar, Actualizar y Borrar devuelven la entidad. Se manda una
            // copia y no la misma referencia, porque el servicio real responde
            // por HTTP y la instancia que vuelve nunca es la que se envio. Una
            // prueba que comparara referencias pasaria por un motivo falso.
            var texto = JsonConvert.SerializeObject(entidad);
            return Task.FromResult(JsonConvert.DeserializeObject<T>(texto)!);
        }
    }
}
