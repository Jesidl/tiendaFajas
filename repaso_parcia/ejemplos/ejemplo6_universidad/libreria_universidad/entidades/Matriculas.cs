using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_universidad.entidades
{
    // Relacion MUCHOS A MUCHOS con datos propios:
    // un estudiante toma muchos cursos y un curso tiene muchos estudiantes.
    // La tabla intermedia (Matriculas) guarda ademas periodo, valores y nota.
    public class Matriculas
    {
        public int Id { get; set; }
        public int Estudiante { get; set; }        // FK -> Estudiantes.Id
        public int Curso { get; set; }             // FK -> Cursos.Id
        public string? Periodo { get; set; }       // "2026-2"
        public DateTime Fecha { get; set; }
        public decimal Valor { get; set; }         // creditos * valorCredito
        public decimal Descuento { get; set; }     // segun estrato
        public decimal Total { get; set; }         // valor - descuento
        public decimal? Nota { get; set; }         // NULL hasta que se califica
        public string? Estado { get; set; }        // Activa | Cancelada | Aprobada | Reprobada

        [ForeignKey("Estudiante")] public Estudiantes? _Estudiante { get; set; }
        [ForeignKey("Curso")] public Cursos? _Curso { get; set; }
    }
}
