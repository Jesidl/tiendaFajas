namespace libreria_gimnacio.nucleo
{
    public class DatosGenerales
    {
        public static string ObtenerStringConexion()
        {
            return "server=localhost\\SQLEXPRESS;database=db_gimnacio;Integrated Security=True;TrustServerCertificate=true;";
        }
    }
}
