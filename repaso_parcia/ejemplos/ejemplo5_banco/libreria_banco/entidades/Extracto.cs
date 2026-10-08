namespace libreria_banco.entidades
{
    // CLASE DE RESULTADO (DTO / reporte).
    // NO tiene DbSet en la Conexion -> NO es tabla, NO sale en la migracion.
    // Solo sirve para devolver un calculo armado por la libreria.
    public class Extracto
    {
        public string? Numero { get; set; }
        public string? Titular { get; set; }
        public string? Tipo { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal TotalDepositos { get; set; }
        public decimal TotalRetiros { get; set; }
        public decimal TotalImpuestos { get; set; }
        public int CantidadMovimientos { get; set; }
        public List<Movimientos>? Movimientos { get; set; }
    }
}
