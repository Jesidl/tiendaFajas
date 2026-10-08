using libreria_notas.entidades;
using libreria_notas.implementaciones;
using libreria_notas.interfaces;
using libreria_notas.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_notas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MateriasController : ControllerBase
    {
        private IMateriasAplicacion IMateriasAplicacion;

        public MateriasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IMateriasAplicacion = new MateriasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Materias> Get()
        {
            return this.IMateriasAplicacion.Consultar();
        }
    }
}
