
using System.ComponentModel.DataAnnotations.Schema;


namespace libreria_hotel.entidades
{
    public class Reservas
    {
        public int Id { get; set; }

        public int Huesped { get; set; }

        public int Habitacion { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime FechaSalida { get; set; }

        public int Personas { get; set; }
        
        public int Noches { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Total { get; set; }

        public decimal Descuento { get; set; }

        public string? Estado { get; set; }
        [ForeignKey("Huesped")] public Huespedes? _Huesped { get; set; }
        [ForeignKey("Habitacion")] public Habitaciones? _Habitacion  { get; set; }
    }
}
