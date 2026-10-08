using libreria_clinica.entidades;
using libreria_clinica.implementaciones;
using libreria_clinica.interfaces;
using libreria_clinica.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MedicosController : ControllerBase
    {
        private IMedicosAplicacion IMedicosAplicacion;

        public MedicosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IMedicosAplicacion = new MedicosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Medicos> Get()
        {
            return this.IMedicosAplicacion.Consultar();
        }

        [HttpPost]
        public Medicos Insertar(Medicos entidad)
        {
            return this.IMedicosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Medicos Actualizar(Medicos entidad)
        {
            return this.IMedicosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Medicos Borrar(Medicos entidad)
        {
            return this.IMedicosAplicacion.Borrar(entidad);
        }
    }
}
