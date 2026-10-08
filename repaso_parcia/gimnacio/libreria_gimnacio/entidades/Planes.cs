

namespace libreria_gimnacio.entidades
{
    public class Planes
    {
        public int Id { get; set; }
        
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }

        public int DuracionDias { get; set; }

        public List<Inscripciones>? Inscripciones { get; set; }
    }
}
