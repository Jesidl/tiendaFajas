namespace libreria_parqueadero.entidades
{
    public class Celdas
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }        // "A-01"
        public string? Tipo { get; set; }          // "Carro" | "Moto" (que vehiculo cabe)
        public bool Ocupada { get; set; }          // la maneja la libreria (entrada/salida)

        public List<Registros>? Registros { get; set; }
    }
}
