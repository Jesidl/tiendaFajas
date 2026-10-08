using System;
using System.Collections.Generic;
using System.Text;

namespace libreria_hotel.entidades
{
    public class Huespedes
    {

        public int Id { get; set; }
        public string? Documento { get; set; }

        public string? Nombre { get; set; }
        public string? Telefono { get; set; }

        public bool Frecuente { get; set; }

        public bool Activo { get; set; }

        public List<Reservas>? Reservas { get; set; }
    }


}
