

namespace libreria_hotel.entidades
{
    public class Habitaciones
    {
        public int Id { get; set; }
        public string? Numero { get; set; }
        public string? Tipo { get; set; }
        public decimal PrecioNoche { get; set; }

        public int Capacidad { get; set; }

        public bool EnMantenimiento { get; set; }

        public List<Reservas>? Reservas { get; set; }
    }
}
