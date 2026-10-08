namespace libreria_universidad.entidades
{
    public class Cursos
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public int Creditos { get; set; }          // 1 a 5
        public int Cupo { get; set; }              // maximo de estudiantes por periodo
        public decimal ValorCredito { get; set; }

        public List<Matriculas>? Matriculas { get; set; }
    }
}
