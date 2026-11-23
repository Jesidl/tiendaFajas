namespace presentacion_tiendaFajas.nucleo
{
    /// <summary>
    /// Direccion del servicio REST.
    ///
    /// Esta aqui y no repetida en cada clase que la necesita porque ahora hay
    /// dos canales hacia el servicio: Comunicaciones, que manda JSON, y
    /// Archivos, que manda multipart. Tener la direccion en un solo lugar
    /// evita que uno de los dos se quede apuntando a un puerto viejo.
    /// </summary>
    public class Servidor
    {
        /// <summary>
        /// Tiene que coincidir con el applicationUrl del perfil http en
        /// Properties/launchSettings.json del proyecto del servicio.
        /// </summary>
        public const string Url = "http://localhost:5250/";

        public static string Ruta(string recurso)
        {
            return Url + recurso.TrimStart('/');
        }
    }
}
