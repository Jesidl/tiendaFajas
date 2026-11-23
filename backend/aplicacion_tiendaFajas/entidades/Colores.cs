using System.ComponentModel.DataAnnotations.Schema;
namespace aplicacion_tiendaFajas.entidades
{
    public class Colores
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo_Hex { get; set; }

        [NotMapped] public List<Variantes>? Variantes { get; set; }
    }
}
