using libreria_clinica.entidades;
using libreria_clinica.implementaciones;
using libreria_clinica.interfaces;
using libreria_clinica.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacientesController : ControllerBase
    {
        private IPacientesAplicacion IPacientesAplicacion;

        public PacientesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IPacientesAplicacion = new PacientesAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Pacientes> Get()
        {
            return this.IPacientesAplicacion.Consultar();
        }

        [HttpPost]
        public Pacientes Insertar(Pacientes entidad)
        {
            return this.IPacientesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Pacientes Actualizar(Pacientes entidad)
        {
            return this.IPacientesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Pacientes Borrar(Pacientes entidad)
        {
            return this.IPacientesAplicacion.Borrar(entidad);
        }
    }
}
