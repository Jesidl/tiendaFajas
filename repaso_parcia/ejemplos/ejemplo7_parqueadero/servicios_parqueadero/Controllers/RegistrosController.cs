using libreria_parqueadero.entidades;
using libreria_parqueadero.implementaciones;
using libreria_parqueadero.interfaces;
using libreria_parqueadero.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_parqueadero.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrosController : ControllerBase
    {
        private IRegistrosAplicacion IRegistrosAplicacion;

        public RegistrosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IRegistrosAplicacion = new RegistrosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Registros> Get()
        {
            return this.IRegistrosAplicacion.Consultar();
        }

        // GET /Registros/Abiertos -> vehiculos que estan adentro
        [HttpGet("Abiertos")]
        public IEnumerable<Registros> Abiertos()
        {
            return this.IRegistrosAplicacion.ConsultarAbiertos();
        }

        // GET /Registros/Recaudo?fecha=2026-10-07   (sin fecha = hoy)
        // DateTime? -> si no viene en la query llega null y usamos hoy
        [HttpGet("Recaudo")]
        public RecaudoDia Recaudo(DateTime? fecha)
        {
            return this.IRegistrosAplicacion.Recaudo(fecha ?? DateTime.Today);
        }

        // GET /Registros/Parcial -> carro nuevo + celda nueva, entro hace 2h 10min
        // Al darle Salida: 131 min -> 3 horas x 4000 = 12000
        [HttpGet("Parcial")]
        public Registros Parcial()
        {
            var entidad = new Registros();
            entidad.Entrada = DateTime.Now.AddHours(-2).AddMinutes(-10);

            string sufijo = (DateTime.Now.Ticks % 1000).ToString("D3");
            entidad._Vehiculo = new Vehiculos();
            entidad._Vehiculo.Placa = "ABC" + sufijo;
            entidad._Vehiculo.Tipo = "Carro";
            entidad._Vehiculo.Propietario = "Pedro Salas";
            entidad._Vehiculo.Mensualidad = false;

            entidad._Celda = new Celdas();
            entidad._Celda.Codigo = "P-" + sufijo;
            entidad._Celda.Tipo = "Carro";

            return this.IRegistrosAplicacion.Insertar(entidad);
        }

        // POST /Registros  { "vehiculo": 1, "celda": 0 } -> celda 0 = asignar automatica
        [HttpPost]
        public Registros Insertar(Registros entidad)
        {
            return this.IRegistrosAplicacion.Insertar(entidad);
        }

        // PUT /Registros/Salida/1
        [HttpPut("Salida/{id}")]
        public Registros Salida(int id)
        {
            return this.IRegistrosAplicacion.Salida(id);
        }

        // PUT /Registros  { "id": 1, "celda": 3 } -> mover de celda
        [HttpPut]
        public Registros Actualizar(Registros entidad)
        {
            return this.IRegistrosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Registros Borrar(Registros entidad)
        {
            return this.IRegistrosAplicacion.Borrar(entidad);
        }
    }
}
