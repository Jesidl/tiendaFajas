namespace aplicacion_tiendaFajas.entidades
{
    // Sin ninguna clave foranea, a proposito: un registro de auditoria debe
    // sobrevivir al borrado de la fila que describe. Por eso los datos van
    // desnormalizados como texto.
    public class Auditorias
    {
        public int Id { get; set; }
        public string? Entidad { get; set; }
        public int Llave { get; set; }
        public string? Operacion { get; set; }
        public string? Usuario { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Datos { get; set; }
    }
}