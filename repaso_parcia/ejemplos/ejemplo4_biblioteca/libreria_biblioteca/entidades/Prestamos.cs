using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_biblioteca.entidades
{
    public class Prestamos
    {
        public int Id { get; set; }
        public int Libro { get; set; }                  // FK -> Libros.Id
        public int Lector { get; set; }                 // FK -> Lectores.Id
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaLimite { get; set; }       // = FechaPrestamo + 15 dias (calculada)

        // DateTime? (con ?) = PUEDE SER NULL -> columna "datetime2 NULL".
        // Mientras el libro no se devuelva, no hay fecha de devolucion.
        public DateTime? FechaDevolucion { get; set; }

        public int DiasRetraso { get; set; }
        public decimal Multa { get; set; }
        public bool Devuelto { get; set; }

        [ForeignKey("Libro")] public Libros? _Libro { get; set; }
        [ForeignKey("Lector")] public Lectores? _Lector { get; set; }
    }
}
