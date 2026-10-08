using libreria_gimnacio.entidades;
using libreria_gimnacio.implementaciones;
using libreria_gimnacio.interfaces;
using libreria_gimnacio.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InscripcionesController : ControllerBase
    {
        private IInscripcionesAplicacion IInscripcionesAplicacion;

        public InscripcionesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IInscripcionesAplicacion = new InscripcionesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Inscripciones> Get()
        {
            return this.IInscripcionesAplicacion.Consultar();
        }


        [HttpPost]
        public Inscripciones Insertar(Inscripciones entidad)
        {
            return this.IInscripcionesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Inscripciones Actualizar(Inscripciones entidad)
        {
            return this.IInscripcionesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Inscripciones Borrar(Inscripciones entidad)
        {
            return this.IInscripcionesAplicacion.Borrar(entidad);
        }
    }
}
