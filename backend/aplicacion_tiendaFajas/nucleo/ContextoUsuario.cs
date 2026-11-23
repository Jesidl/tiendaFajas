namespace aplicacion_tiendaFajas.nucleo
{
    /// <summary>
    /// Lleva el usuario de la peticion en curso hasta Conexion, que es quien
    /// escribe la auditoria.
    ///
    /// Usa AsyncLocal y no static simple: AsyncLocal guarda un valor por
    /// flujo de ejecucion asincrono, asi que dos peticiones atendidas a la
    /// vez por el mismo proceso no se pisan el usuario. Un campo static
    /// normal seria compartido por todo el proceso y la auditoria quedaria
    /// atribuida a quien no fue.
    ///
    /// Lo llena el middleware de servicio_tiendaFajas leyendo la cabecera
    /// X-Usuario que envia Comunicaciones.
    /// </summary>
    public class ContextoUsuario
    {
        private static readonly AsyncLocal<string?> actual = new();

        public const string Cabecera = "X-Usuario";
        public const string Anonimo = "sistema";

        public static string? Usuario
        {
            get { return actual.Value; }
            set { actual.Value = value; }
        }

        public static string Nombre()
        {
            var valor = actual.Value;
            return string.IsNullOrEmpty(valor) ? Anonimo : valor;
        }
    }
}
