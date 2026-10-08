using libreria_hotel.entidades;
using libreria_hotel.implementaciones;
using libreria_hotel.interfaces;
using libreria_hotel.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HuespedesController : ControllerBase
    {
        private IHuespedesAplicacion IHuespedesAplicacion;

        public HuespedesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IHuespedesAplicacion = new HuespedesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Huespedes> Get()
        {
            return this.IHuespedesAplicacion.Consultar();
        }

        [HttpPost]
        public Huespedes Insertar(Huespedes entidad)
        {
            return this.IHuespedesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Huespedes Actualizar(Huespedes entidad)
        {
            return this.IHuespedesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Huespedes Borrar(Huespedes entidad)
        {
            return this.IHuespedesAplicacion.Borrar(entidad);
        }
    }
}
