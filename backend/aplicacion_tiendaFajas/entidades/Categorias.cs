using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Categorias
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }

        [NotMapped] public List<Productos>? Productos { get; set; }
    }
}
