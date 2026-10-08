using System;
using System.Collections.Generic;
using System.Text;

namespace libreria_tienda.entidades
{
    public class Clientes
    {

        public int Id { get; set; }
        public string? Cedula { get; set; }

        public string? Nombre { get; set; }
        public string? Telefono { get; set; }

        public bool Activo { get; set; }

        public List<Ventas>? Ventas { get; set; }
    }


}
