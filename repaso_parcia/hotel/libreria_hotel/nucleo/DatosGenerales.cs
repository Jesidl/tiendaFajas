namespace libreria_hotel.nucleo
{
    public class DatosGenerales
    {
        public static string ObtenerStringConexion()
        {
            return "server=localhost\\SQLEXPRESS;database=db_hotel;Integrated Security=True;TrustServerCertificate=true;";
        }
    }
}
