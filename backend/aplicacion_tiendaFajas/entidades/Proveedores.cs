using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Proveedores
    {
        public int Id { get; set; }
        public string? Nit { get; set; }
        public string? Nombre_Empresa { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        [NotMapped] public List<Pedidos>? Pedidos { get; set; }
    }
}
