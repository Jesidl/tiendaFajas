using libreria_tienda.entidades;
using libreria_tienda.implementaciones;
using libreria_tienda.interfaces;
using libreria_tienda.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VentasController : ControllerBase
    {
        private IVentasAplicacion IVentasAplicacion;

        public VentasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IVentasAplicacion = new VentasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Ventas> Get()
        {
            return this.IVentasAplicacion.Consultar();
        }

        // http://localhost:XXXX/Ventas/Parcial
        [HttpGet("Parcial")]
        public Ventas Parcial()
        {
            var entidad = new Ventas();
            entidad.Fecha = DateTime.Now;
            entidad.Cantidad = 3;

            entidad._Cliente = new Clientes();
            entidad._Cliente.Cedula = "1001";
            entidad._Cliente.Nombre = "Pepito";
            entidad._Cliente.Telefono = "3001234567";
            entidad._Cliente.Activo = true;

            entidad._Producto = new Productos();
            entidad._Producto.Codigo = "P001";
            entidad._Producto.Nombre = "Faja";
            entidad._Producto.Precio = 50000;

            return this.IVentasAplicacion.Insertar(entidad);
        }


        [HttpPost]
        public Ventas Insertar(Ventas entidad)
        {
            return this.IVentasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Ventas Actualizar(Ventas entidad)
        {
            return this.IVentasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Ventas Borrar(Ventas entidad)
        {
            return this.IVentasAplicacion.Borrar(entidad);
        }
    }
}
