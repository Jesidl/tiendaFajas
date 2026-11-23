using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class Metodos_PagoController : ControllerBase
    {
        private IMetodos_PagoAplicacion IMetodos_PagoAplicacion;

        public Metodos_PagoController(IMetodos_PagoAplicacion IMetodos_PagoAplicacion)
        {
            this.IMetodos_PagoAplicacion = IMetodos_PagoAplicacion;
        }

        [HttpPost]
        public List<Metodos_Pago> Consultar()
        {
            return this.IMetodos_PagoAplicacion.Consultar();
        }

        [HttpPost]
        public List<Metodos_Pago> Buscar(Metodos_Pago entidad)
        {
            return this.IMetodos_PagoAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Metodos_Pago Insertar(Metodos_Pago entidad)
        {
            return this.IMetodos_PagoAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Metodos_Pago Actualizar(Metodos_Pago entidad)
        {
            return this.IMetodos_PagoAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Metodos_Pago Borrar(Metodos_Pago entidad)
        {
            return this.IMetodos_PagoAplicacion.Borrar(entidad);
        }
    }
}