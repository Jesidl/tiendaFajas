using libreria_parqueadero.entidades;
using libreria_parqueadero.implementaciones;
using libreria_parqueadero.interfaces;
using libreria_parqueadero.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_parqueadero.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehiculosController : ControllerBase
    {
        private IVehiculosAplicacion IVehiculosAplicacion;

        public VehiculosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IVehiculosAplicacion = new VehiculosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Vehiculos> Get()
        {
            return this.IVehiculosAplicacion.Consultar();
        }

        [HttpPost]
        public Vehiculos Insertar(Vehiculos entidad)
        {
            return this.IVehiculosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Vehiculos Actualizar(Vehiculos entidad)
        {
            return this.IVehiculosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Vehiculos Borrar(Vehiculos entidad)
        {
            return this.IVehiculosAplicacion.Borrar(entidad);
        }
    }
}
