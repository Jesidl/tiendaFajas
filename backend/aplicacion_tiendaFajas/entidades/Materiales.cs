using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Materiales
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        [NotMapped] public List<Productos>? Productos { get; set; }
    }
}
