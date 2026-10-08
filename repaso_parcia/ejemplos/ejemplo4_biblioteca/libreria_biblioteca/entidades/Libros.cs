namespace libreria_biblioteca.entidades
{
    public class Libros
    {
        public int Id { get; set; }
        public string? Isbn { get; set; }
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public int Ejemplares { get; set; }    // total de copias que tiene la biblioteca
        public int Disponibles { get; set; }   // copias en estante (CAMPO DERIVADO: lo maneja la libreria)

        public List<Prestamos>? Prestamos { get; set; }
    }
}
