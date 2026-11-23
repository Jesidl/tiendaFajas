using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MedidasController : ControllerBase
    {
        private IMedidasAplicacion IMedidasAplicacion;

        public MedidasController(IMedidasAplicacion IMedidasAplicacion)
        {
            this.IMedidasAplicacion = IMedidasAplicacion;
        }

        [HttpPost]
        public List<Medidas> Consultar()
        {
            return this.IMedidasAplicacion.Consultar();
        }

        [HttpPost]
        public List<Medidas> Buscar(Medidas entidad)
        {
            return this.IMedidasAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Medidas Insertar(Medidas entidad)
        {
            return this.IMedidasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Medidas Actualizar(Medidas entidad)
        {
            return this.IMedidasAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Medidas Borrar(Medidas entidad)
        {
            return this.IMedidasAplicacion.Borrar(entidad);
        }
    }
}