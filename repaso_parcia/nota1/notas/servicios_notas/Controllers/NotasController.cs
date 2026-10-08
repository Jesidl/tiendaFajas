using libreria_notas.entidades;
using libreria_notas.implementaciones;
using libreria_notas.interfaces;
using libreria_notas.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_notas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotasController : ControllerBase
    {
        private INotasAplicacion INotasAplicacion;

        public NotasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.INotasAplicacion = new NotasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Notas> Get()
        {
            return this.INotasAplicacion.Consultar();
        }
    }
}
