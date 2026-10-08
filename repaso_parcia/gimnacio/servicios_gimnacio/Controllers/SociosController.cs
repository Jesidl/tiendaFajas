using libreria_gimnacio.entidades;
using libreria_gimnacio.implementaciones;
using libreria_gimnacio.interfaces;
using libreria_gimnacio.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_gimnacio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SociosController : ControllerBase
    {
        private ISociosAplicacion ISociosAplicacion;

        public SociosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ISociosAplicacion = new SociosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Socios> Get()
        {
            return this.ISociosAplicacion.Consultar();
        }

        [HttpPost]
        public Socios Insertar(Socios  entidad)
        {
            return this.ISociosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Socios Actualizar(Socios entidad)
        {
            return this.ISociosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Socios Borrar(Socios entidad)
        {
            return this.ISociosAplicacion.Borrar(entidad);
        }
    }
}
