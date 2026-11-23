using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ColoresController : ControllerBase
    {
        private IColoresAplicacion IColoresAplicacion;

        public ColoresController(IColoresAplicacion IColoresAplicacion)
        {
            this.IColoresAplicacion = IColoresAplicacion;
        }

        [HttpPost]
        public List<Colores> Consultar()
        {
            return this.IColoresAplicacion.Consultar();
        }

        [HttpPost]
        public List<Colores> Buscar(Colores entidad)
        {
            return this.IColoresAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Colores Insertar(Colores entidad)
        {
            return this.IColoresAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Colores Actualizar(Colores entidad)
        {
            return this.IColoresAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Colores Borrar(Colores entidad)
        {
            return this.IColoresAplicacion.Borrar(entidad);
        }
    }
}