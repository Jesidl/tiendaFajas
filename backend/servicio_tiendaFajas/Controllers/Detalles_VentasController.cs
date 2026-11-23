using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class Detalles_VentasController : ControllerBase
    {
        private IDetalles_VentasAplicacion IDetalles_VentasAplicacion;

        public Detalles_VentasController(IDetalles_VentasAplicacion IDetalles_VentasAplicacion)
        {
            this.IDetalles_VentasAplicacion = IDetalles_VentasAplicacion;
        }

        [HttpPost]
        public List<Detalles_Ventas> Consultar()
        {
            return this.IDetalles_VentasAplicacion.Consultar();
        }

        [HttpPost]
        public List<Detalles_Ventas> Buscar(Detalles_Ventas entidad)
        {
            return this.IDetalles_VentasAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Detalles_Ventas Insertar(Detalles_Ventas entidad)
        {
            return this.IDetalles_VentasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Detalles_Ventas Actualizar(Detalles_Ventas entidad)
        {
            return this.IDetalles_VentasAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Detalles_Ventas Borrar(Detalles_Ventas entidad)
        {
            return this.IDetalles_VentasAplicacion.Borrar(entidad);
        }
    }
}