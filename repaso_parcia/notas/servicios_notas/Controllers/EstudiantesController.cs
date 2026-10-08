using libreria_notas.entidades;
using libreria_notas.implementaciones;
using libreria_notas.interfaces;
using libreria_notas.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_notas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private IEstudiantesAplicacion IEstudiantesAplicacion;

        public EstudiantesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IEstudiantesAplicacion = new EstudiantesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Estudiantes> Get()
        {
            return this.IEstudiantesAplicacion.Consultar();
        }

        [HttpPost]
        public Estudiantes Insertar(Estudiantes entidad)
        {
            return this.IEstudiantesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Estudiantes Actualizar(Estudiantes entidad)
        {
            return this.IEstudiantesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Estudiantes Borrar(Estudiantes entidad)
        {
            return this.IEstudiantesAplicacion.Borrar(entidad);
        }
    }
}
