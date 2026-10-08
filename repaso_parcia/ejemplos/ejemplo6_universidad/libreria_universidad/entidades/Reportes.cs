namespace libreria_universidad.entidades
{
    // DTOs de reportes: SIN DbSet -> no son tablas.
    // Se pueden poner varias clases pequenas en un mismo archivo.

    public class ResumenCurso
    {
        public string? Curso { get; set; }
        public int Cupo { get; set; }
        public int Inscritos { get; set; }
        public int CuposDisponibles { get; set; }
        public decimal Recaudo { get; set; }
        public decimal? Promedio { get; set; }   // null si nadie ha sido calificado
    }

    public class PromedioEstudiante
    {
        public string? Estudiante { get; set; }
        public int CursosCalificados { get; set; }
        public int CreditosAprobados { get; set; }
        public decimal PromedioPonderado { get; set; }
    }
}
