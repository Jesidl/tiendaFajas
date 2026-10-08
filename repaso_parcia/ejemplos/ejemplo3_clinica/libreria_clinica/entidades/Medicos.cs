namespace libreria_clinica.entidades
{
    public class Medicos
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Especialidad { get; set; }
        public decimal Tarifa { get; set; }   // valor base de la cita
        public bool Activo { get; set; }

        public List<Citas>? Citas { get; set; }
    }
}
