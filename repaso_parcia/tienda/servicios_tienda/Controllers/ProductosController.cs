using libreria_tienda.entidades;
using libreria_tienda.implementaciones;
using libreria_tienda.interfaces;
using libreria_tienda.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosController : ControllerBase
    {
        private IProductosAplicacion IProductosAplicacion;

        public ProductosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IProductosAplicacion = new ProductosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Productos> Get()
        {
            return this.IProductosAplicacion.Consultar();
        }

        [HttpPost]
        public Productos Insertar(Productos entidad)
        {
            return this.IProductosAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Productos Actualizar(Productos entidad)
        {
            return this.IProductosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Productos Borrar(Productos entidad)
        {
            return this.IProductosAplicacion.Borrar(entidad);
        }
    }
}
