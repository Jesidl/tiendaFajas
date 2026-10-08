using libreria_parqueadero.entidades;
using libreria_parqueadero.implementaciones;
using libreria_parqueadero.interfaces;
using libreria_parqueadero.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_parqueadero.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CeldasController : ControllerBase
    {
        private ICeldasAplicacion ICeldasAplicacion;

        public CeldasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ICeldasAplicacion = new CeldasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Celdas> Get()
        {
            return this.ICeldasAplicacion.Consultar();
        }

        // GET /Celdas/Disponibles/Carro   (parametro string en la ruta)
        [HttpGet("Disponibles/{tipo}")]
        public IEnumerable<Celdas> Disponibles(string tipo)
        {
            return this.ICeldasAplicacion.ConsultarDisponibles(tipo);
        }

        [HttpPost]
        public Celdas Insertar(Celdas entidad)
        {
            return this.ICeldasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Celdas Actualizar(Celdas entidad)
        {
            return this.ICeldasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Celdas Borrar(Celdas entidad)
        {
            return this.ICeldasAplicacion.Borrar(entidad);
        }
    }
}
