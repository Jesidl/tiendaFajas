using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Empleados
    {
        public int Id { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public int Sucursal { get; set; }
        public int Cargo { get; set; }

        [ForeignKey("Sucursal")] public Sucursales? _Sucursal { get; set; }
        [ForeignKey("Cargo")] public Cargos? _Cargo { get; set; }
        [NotMapped] public List<Ventas>? Ventas { get; set; }
        [NotMapped] public List<Pedidos>? Pedidos { get; set; }
    }
}
