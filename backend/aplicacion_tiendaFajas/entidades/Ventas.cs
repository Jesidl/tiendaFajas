using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Ventas
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public int Cliente { get; set; }
        public int Empleado { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal Total { get; set; }

        [ForeignKey("Cliente")] public Clientes? _Cliente { get; set; }
        [ForeignKey("Empleado")] public Empleados? _Empleado { get; set; }
        [NotMapped] public List<Detalles_Ventas>? Detalles_Ventas { get; set; }
        [NotMapped] public List<Pagos>? Pagos { get; set; }
    }
}
