using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MaterialesController : ControllerBase
    {
        private IMaterialesAplicacion IMaterialesAplicacion;

        public MaterialesController(IMaterialesAplicacion IMaterialesAplicacion)
        {
            this.IMaterialesAplicacion = IMaterialesAplicacion;
        }

        [HttpPost]
        public List<Materiales> Consultar()
        {
            return this.IMaterialesAplicacion.Consultar();
        }

        [HttpPost]
        public List<Materiales> Buscar(Materiales entidad)
        {
            return this.IMaterialesAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Materiales Insertar(Materiales entidad)
        {
            return this.IMaterialesAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Materiales Actualizar(Materiales entidad)
        {
            return this.IMaterialesAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Materiales Borrar(Materiales entidad)
        {
            return this.IMaterialesAplicacion.Borrar(entidad);
        }
    }
}