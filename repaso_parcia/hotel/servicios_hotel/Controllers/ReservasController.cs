using libreria_hotel.entidades;
using libreria_hotel.implementaciones;
using libreria_hotel.interfaces;
using libreria_hotel.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservasController : ControllerBase
    {
        private IReservasAplicacion IReservasAplicacion;

        public ReservasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IReservasAplicacion = new ReservasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Reservas> Get()
        {
            return this.IReservasAplicacion.Consultar();
        }



        [HttpPost]
        public Reservas Insertar(Reservas entidad)
        {
            return this.IReservasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Reservas Actualizar(Reservas entidad)
        {
            return this.IReservasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Reservas Borrar(Reservas entidad)
        {
            return this.IReservasAplicacion.Borrar(entidad);
        }
    }
}
