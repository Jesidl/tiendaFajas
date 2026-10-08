using libreria_hotel.entidades;
using libreria_hotel.implementaciones;
using libreria_hotel.interfaces;
using libreria_hotel.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HabitacionesController : ControllerBase
    {
        private IHabitacionesAplicacion IHabitacionesAplicacion;

        public HabitacionesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IHabitacionesAplicacion = new HabitacionesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Habitaciones> Get()
        {
            return this.IHabitacionesAplicacion.Consultar();
        }

        [HttpPost]
        public Habitaciones Insertar(Habitaciones entidad)
        {
            return this.IHabitacionesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Habitaciones Actualizar(Habitaciones entidad)
        {
            return this.IHabitacionesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Habitaciones Borrar(Habitaciones entidad)
        {
            return this.IHabitacionesAplicacion.Borrar(entidad);
        }
    }
}
