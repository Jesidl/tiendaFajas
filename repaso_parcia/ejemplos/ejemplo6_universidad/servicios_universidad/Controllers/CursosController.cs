using libreria_universidad.entidades;
using libreria_universidad.implementaciones;
using libreria_universidad.interfaces;
using libreria_universidad.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_universidad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CursosController : ControllerBase
    {
        private ICursosAplicacion ICursosAplicacion;

        public CursosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ICursosAplicacion = new CursosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Cursos> Get()
        {
            return this.ICursosAplicacion.Consultar();
        }

        [HttpPost]
        public Cursos Insertar(Cursos entidad)
        {
            return this.ICursosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Cursos Actualizar(Cursos entidad)
        {
            return this.ICursosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Cursos Borrar(Cursos entidad)
        {
            return this.ICursosAplicacion.Borrar(entidad);
        }
    }
}
