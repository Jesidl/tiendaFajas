

namespace libreria_tienda.entidades
{
    public class Productos
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }

        public List<Ventas>? Ventas { get; set; }
    }
}
