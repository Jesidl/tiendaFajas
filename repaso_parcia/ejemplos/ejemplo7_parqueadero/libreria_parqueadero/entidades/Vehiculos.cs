namespace libreria_parqueadero.entidades
{
    public class Vehiculos
    {
        public int Id { get; set; }
        public string? Placa { get; set; }         // se guarda NORMALIZADA: sin espacios y en MAYUSCULA
        public string? Tipo { get; set; }          // "Carro" | "Moto"
        public string? Propietario { get; set; }
        public bool Mensualidad { get; set; }      // si paga mensualidad, no paga por horas

        public List<Registros>? Registros { get; set; }
    }
}
