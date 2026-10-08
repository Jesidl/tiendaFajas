using libreria_gimnacio.entidades;
using libreria_gimnacio.implementaciones;
using libreria_gimnacio.interfaces;
using libreria_gimnacio.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_gimnacio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanesController : ControllerBase
    {
        private IPlanesAplicacion IPlanesAplicacion;

        public PlanesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IPlanesAplicacion = new PlanesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Planes> Get()
        {
            return this.IPlanesAplicacion.Consultar();
        }

        [HttpPost]
        public Planes Insertar(Planes entidad)
        {
            return this.IPlanesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Planes Actualizar(Planes entidad)
        {
            return this.IPlanesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Planes Borrar(Planes entidad)
        {
            return this.IPlanesAplicacion.Borrar(entidad);
        }
    }
}
