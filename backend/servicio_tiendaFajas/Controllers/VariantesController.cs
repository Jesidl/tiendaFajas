using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VariantesController : ControllerBase
    {
        private IVariantesAplicacion IVariantesAplicacion;

        public VariantesController(IVariantesAplicacion IVariantesAplicacion)
        {
            this.IVariantesAplicacion = IVariantesAplicacion;
        }

        [HttpPost]
        public List<Variantes> Consultar()
        {
            return this.IVariantesAplicacion.Consultar();
        }

        [HttpPost]
        public List<Variantes> Buscar(Variantes entidad)
        {
            return this.IVariantesAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Variantes Insertar(Variantes entidad)
        {
            return this.IVariantesAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Variantes Actualizar(Variantes entidad)
        {
            return this.IVariantesAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Variantes Borrar(Variantes entidad)
        {
            return this.IVariantesAplicacion.Borrar(entidad);
        }
    }
}