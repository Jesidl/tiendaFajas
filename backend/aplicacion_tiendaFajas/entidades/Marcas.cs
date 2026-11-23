using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Marcas
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Pais { get; set; }
        public bool Estado { get; set; }

        [NotMapped] public List<Productos>? Productos { get; set; }
    }
}
