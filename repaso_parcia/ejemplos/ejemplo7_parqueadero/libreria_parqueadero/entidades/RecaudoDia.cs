namespace libreria_parqueadero.entidades
{
    // DTO con OTRO DTO adentro (lista). Ninguno es tabla.
    public class RecaudoDia
    {
        public DateTime Fecha { get; set; }
        public int TotalVehiculos { get; set; }
        public decimal TotalGeneral { get; set; }
        public List<RecaudoTipo>? PorTipo { get; set; }
    }

    public class RecaudoTipo
    {
        public string? Tipo { get; set; }
        public int Vehiculos { get; set; }
        public int MinutosTotales { get; set; }
        public decimal Total { get; set; }
    }
}
