namespace libreria_parqueadero.nucleo
{
    public class DatosGenerales
    {
        // Cadena de conexion a SQL Server.
        //  - En este PC: localhost\\SQLEXPRESS (doble \\ porque en C# \ es caracter especial).
        //  - En el examen: revisar el nombre del servidor (casi siempre "localhost").
        public static string ObtenerStringConexion()
        {
            return "server=localhost\\SQLEXPRESS;database=db_parqueadero;Integrated Security=True;TrustServerCertificate=true;";
        }
    }
}
