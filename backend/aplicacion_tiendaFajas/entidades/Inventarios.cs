using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Inventarios
    {
        public int Id { get; set; }
        public int Variante { get; set; }
        public int Sucursal { get; set; }
        public int Cantidad { get; set; }
        public int Stock_Minimo { get; set; }
        public DateTime? Fecha_Actualizacion { get; set; }

        [NotMapped] public string? Filtro { get; set; }

        [ForeignKey("Variante")] public Variantes? _Variante { get; set; }
        [ForeignKey("Sucursal")] public Sucursales? _Sucursal { get; set; }
    }
}
