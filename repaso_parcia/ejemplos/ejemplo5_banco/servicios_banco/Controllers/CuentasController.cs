using libreria_banco.entidades;
using libreria_banco.implementaciones;
using libreria_banco.interfaces;
using libreria_banco.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_banco.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CuentasController : ControllerBase
    {
        private ICuentasAplicacion ICuentasAplicacion;

        public CuentasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ICuentasAplicacion = new CuentasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Cuentas> Get()
        {
            return this.ICuentasAplicacion.Consultar();
        }

        // GET /Cuentas/PorCliente/1
        [HttpGet("PorCliente/{cliente}")]
        public IEnumerable<Cuentas> PorCliente(int cliente)
        {
            return this.ICuentasAplicacion.ConsultarPorCliente(cliente);
        }

        // GET /Cuentas/Parcial -> cliente nuevo + cuenta de ahorros con 500000
        // (se crea automaticamente el movimiento de "Apertura")
        [HttpGet("Parcial")]
        public Cuentas Parcial()
        {
            var entidad = new Cuentas();
            entidad.Tipo = "Ahorros";
            entidad.Saldo = 500000;

            entidad._Cliente = new Clientes();
            entidad._Cliente.Documento = "C-" + DateTime.Now.Ticks;
            entidad._Cliente.Nombre = "Laura Diaz";
            entidad._Cliente.Telefono = "3115550000";

            return this.ICuentasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Cuentas Insertar(Cuentas entidad)
        {
            return this.ICuentasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Cuentas Actualizar(Cuentas entidad)
        {
            return this.ICuentasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Cuentas Borrar(Cuentas entidad)
        {
            return this.ICuentasAplicacion.Borrar(entidad);
        }
    }
}
