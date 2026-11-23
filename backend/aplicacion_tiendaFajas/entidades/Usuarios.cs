using System.ComponentModel.DataAnnotations.Schema;

namespace aplicacion_tiendaFajas.entidades
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string? Correo { get; set; }
        public string? Nombre { get; set; }
        public string? Clave { get; set; }
        public int Rol { get; set; }
        public bool Estado { get; set; }

        [ForeignKey("Rol")] public Roles? _Rol { get; set; }
    }
}