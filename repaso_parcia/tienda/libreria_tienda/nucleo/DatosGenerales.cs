namespace libreria_tienda.nucleo
{
    public class DatosGenerales
    {
        public static string ObtenerStringConexion()
        {
            return "server=localhost\\SQLEXPRESS;database=db_tienda;Integrated Security=True;TrustServerCertificate=true;";
        }
    }
}
