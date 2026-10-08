using libreria_biblioteca.entidades;
using libreria_biblioteca.implementaciones;
using libreria_biblioteca.interfaces;
using libreria_biblioteca.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LectoresController : ControllerBase
    {
        private ILectoresAplicacion ILectoresAplicacion;

        public LectoresController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ILectoresAplicacion = new LectoresAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Lectores> Get()
        {
            return this.ILectoresAplicacion.Consultar();
        }

        [HttpPost]
        public Lectores Insertar(Lectores entidad)
        {
            return this.ILectoresAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Lectores Actualizar(Lectores entidad)
        {
            return this.ILectoresAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Lectores Borrar(Lectores entidad)
        {
            return this.ILectoresAplicacion.Borrar(entidad);
        }
    }
}
