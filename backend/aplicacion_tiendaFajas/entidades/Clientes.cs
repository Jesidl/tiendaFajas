using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Clientes
    {
        public int Id { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }

        [NotMapped] public List<Medidas>? Medidas { get; set; }
        [NotMapped] public List<Ventas>? Ventas { get; set; }
    }
}
