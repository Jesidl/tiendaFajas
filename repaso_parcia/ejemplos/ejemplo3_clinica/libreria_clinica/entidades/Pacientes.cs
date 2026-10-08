namespace libreria_clinica.entidades
{
    public class Pacientes
    {
        public int Id { get; set; }
        public string? Documento { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }   // se usa para calcular la EDAD
        public bool Subsidiado { get; set; }            // regla de descuento
        public bool Activo { get; set; }

        // Lado "muchos" de la relacion: un paciente tiene muchas citas
        public List<Citas>? Citas { get; set; }
    }
}
