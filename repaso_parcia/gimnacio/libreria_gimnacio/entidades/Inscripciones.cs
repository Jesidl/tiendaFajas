
using System.ComponentModel.DataAnnotations.Schema;


namespace libreria_gimnacio.entidades
{
    public class Inscripciones
    {
        public int Id { get; set; }

        public int Socio { get; set; }

        public int Plan { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int Valor { get; set; }

        public decimal Descuento { get; set; }

        public decimal Total { get; set; }

        [ForeignKey("Socio")] public Socios? _Socio { get; set; }
        [ForeignKey("Plan")] public Planes? _Plan { get; set; }
    }
}
