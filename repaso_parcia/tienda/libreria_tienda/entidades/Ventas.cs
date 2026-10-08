
using System.ComponentModel.DataAnnotations.Schema;


namespace libreria_tienda.entidades
{
    public class Ventas
    {
        public int Id { get; set; }

        public int Cliente { get; set; }

        public int Producto { get; set; }

        public DateTime Fecha { get; set; }

        public int Cantidad { get; set; }

        public decimal Total { get; set; }

        [ForeignKey("Cliente")] public Clientes? _Cliente { get; set; }
        [ForeignKey("Producto")] public Productos? _Producto { get; set; }
    }
}
