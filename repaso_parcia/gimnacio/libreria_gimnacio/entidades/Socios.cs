

namespace libreria_gimnacio.entidades
{
    public class Socios
    {

        public int Id { get; set; }
        public string? Documento { get; set; }

        public string? Nombre { get; set; }
        public bool Estudiante { get; set; }

        public bool Activo { get; set; }

        public List<Inscripciones>? Inscripciones { get; set; }
    }


}
