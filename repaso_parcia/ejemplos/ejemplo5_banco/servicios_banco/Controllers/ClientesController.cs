using libreria_banco.entidades;
using libreria_banco.implementaciones;
using libreria_banco.interfaces;
using libreria_banco.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_banco.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : ControllerBase
    {
        private IClientesAplicacion IClientesAplicacion;

        public ClientesController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IClientesAplicacion = new ClientesAplicacion(conexion);
        }

        // GET /Clientes -> cada cliente con su lista de cuentas
        [HttpGet]
        public IEnumerable<Clientes> Get()
        {
            return this.IClientesAplicacion.Consultar();
        }

        [HttpPost]
        public Clientes Insertar(Clientes entidad)
        {
            return this.IClientesAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Clientes Actualizar(Clientes entidad)
        {
            return this.IClientesAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Clientes Borrar(Clientes entidad)
        {
            return this.IClientesAplicacion.Borrar(entidad);
        }
    }
}
