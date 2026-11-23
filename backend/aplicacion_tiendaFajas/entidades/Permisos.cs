using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Permisos
    {
        public int Id { get; set; }
        public int Rol { get; set; }
        public string? Modulo { get; set; }
        public bool Consultar { get; set; }
        public bool Insertar { get; set; }
        public bool Actualizar { get; set; }
        public bool Borrar { get; set; }

        [ForeignKey("Rol")] public Roles? _Rol { get; set; }
    }
}