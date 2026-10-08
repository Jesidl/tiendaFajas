namespace libreria_notas.entidades
{
    public class Materias
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Programa { get; set; }

        public List<Notas>? Notas { get; set; }
    }
}