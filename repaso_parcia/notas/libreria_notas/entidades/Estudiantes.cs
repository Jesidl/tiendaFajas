namespace libreria_notas.entidades
{
    public class Estudiantes
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombres { get; set; }
        public DateTime Fecha { get; set; }
        public bool Estado { get; set; }

        public List<Notas>? Notas { get; set; }
    }
}