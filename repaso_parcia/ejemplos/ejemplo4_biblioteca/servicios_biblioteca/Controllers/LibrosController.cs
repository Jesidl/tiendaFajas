using libreria_biblioteca.entidades;
using libreria_biblioteca.implementaciones;
using libreria_biblioteca.interfaces;
using libreria_biblioteca.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibrosController : ControllerBase
    {
        private ILibrosAplicacion ILibrosAplicacion;

        public LibrosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ILibrosAplicacion = new LibrosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Libros> Get()
        {
            return this.ILibrosAplicacion.Consultar();
        }

        // GET /Libros/Disponibles
        [HttpGet("Disponibles")]
        public IEnumerable<Libros> Disponibles()
        {
            return this.ILibrosAplicacion.ConsultarDisponibles();
        }

        [HttpPost]
        public Libros Insertar(Libros entidad)
        {
            return this.ILibrosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Libros Actualizar(Libros entidad)
        {
            return this.ILibrosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Libros Borrar(Libros entidad)
        {
            return this.ILibrosAplicacion.Borrar(entidad);
        }
    }
}
