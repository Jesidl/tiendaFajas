namespace presentacion_tiendaFajas.nucleo
{
    /// <summary>
    /// Guarda el usuario de la peticion que esta atendiendo el front, para que
    /// Comunicaciones lo pueda enviar al servicio en la cabecera X-Usuario.
    ///
    /// Igual que ContextoUsuario del lado del servicio, usa AsyncLocal y no un
    /// static simple: dos usuarios navegando a la vez comparten el proceso del
    /// front, y un static los mezclaria.
    ///
    /// Lo llena el metodo Sesion() de cada PageModel.
    /// </summary>
    public class SesionActual
    {
        private static readonly AsyncLocal<string?> actual = new();

        public static string? Usuario
        {
            get { return actual.Value; }
            set { actual.Value = value; }
        }
    }
}
