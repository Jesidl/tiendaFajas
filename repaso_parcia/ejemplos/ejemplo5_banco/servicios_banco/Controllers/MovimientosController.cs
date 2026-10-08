using libreria_banco.entidades;
using libreria_banco.implementaciones;
using libreria_banco.interfaces;
using libreria_banco.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_banco.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovimientosController : ControllerBase
    {
        private IMovimientosAplicacion IMovimientosAplicacion;

        public MovimientosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IMovimientosAplicacion = new MovimientosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Movimientos> Get()
        {
            return this.IMovimientosAplicacion.Consultar();
        }

        [HttpGet("PorCuenta/{cuenta}")]
        public IEnumerable<Movimientos> PorCuenta(int cuenta)
        {
            return this.IMovimientosAplicacion.ConsultarPorCuenta(cuenta);
        }

        // GET /Movimientos/Extracto/1 -> devuelve el DTO Extracto
        [HttpGet("Extracto/{cuenta}")]
        public Extracto Extracto(int cuenta)
        {
            return this.IMovimientosAplicacion.Extracto(cuenta);
        }

        // GET /Movimientos/Parcial/1 -> retira 100000 de la cuenta 1 (impuesto 400)
        [HttpGet("Parcial/{cuenta}")]
        public Movimientos Parcial(int cuenta)
        {
            var entidad = new Movimientos();
            entidad.Cuenta = cuenta;
            entidad.Tipo = "Retiro";
            entidad.Valor = 100000;
            entidad.Descripcion = "Retiro cajero";

            return this.IMovimientosAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Movimientos Insertar(Movimientos entidad)
        {
            return this.IMovimientosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Movimientos Actualizar(Movimientos entidad)
        {
            return this.IMovimientosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Movimientos Borrar(Movimientos entidad)
        {
            return this.IMovimientosAplicacion.Borrar(entidad);
        }
    }
}
