using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_notas.entidades
{
    public class Notas
    {
        public int Id { get; set; }
        public int Materia { get; set; }
        public int Estudiante { get; set; }
        public decimal Nota1 { get; set; }
        public decimal Nota2 { get; set; }
        public decimal Nota3 { get; set; }
        public decimal Nota4 { get; set; }
        public decimal Nota5 { get; set; }
        public decimal NotaFinal { get; set; }

        [ForeignKey("Materia")] public Materias? _Materia { get; set; }
        [ForeignKey("Estudiante")] public Estudiantes? _Estudiante { get; set; }
    }
}