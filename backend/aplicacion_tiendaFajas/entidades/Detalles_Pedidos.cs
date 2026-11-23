using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Detalles_Pedidos
    {
        public int Id { get; set; }
        public int Pedido { get; set; }
        public int Variante { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio_Compra { get; set; }
        public decimal Subtotal { get; set; }

        [NotMapped] public string? Filtro { get; set; }

        [ForeignKey("Pedido")] public Pedidos? _Pedido { get; set; }
        [ForeignKey("Variante")] public Variantes? _Variante { get; set; }
    }
}
