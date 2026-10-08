namespace libreria_biblioteca.entidades
{
    public class Lectores
    {
        public int Id { get; set; }
        public string? Documento { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public bool Activo { get; set; }

        public List<Prestamos>? Prestamos { get; set; }
    }
}
