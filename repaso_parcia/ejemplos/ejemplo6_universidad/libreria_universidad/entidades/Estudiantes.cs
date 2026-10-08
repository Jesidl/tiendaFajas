namespace libreria_universidad.entidades
{
    public class Estudiantes
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public int Estrato { get; set; }   // 1 a 6 -> define el descuento
        public bool Activo { get; set; }

        public List<Matriculas>? Matriculas { get; set; }
    }
}
