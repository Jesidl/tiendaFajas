using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Sucursales
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Telefono { get; set; }
        public bool Estado { get; set; }

        [NotMapped] public List<Empleados>? Empleados { get; set; }
        [NotMapped] public List<Inventarios>? Inventarios { get; set; }
    }
}
