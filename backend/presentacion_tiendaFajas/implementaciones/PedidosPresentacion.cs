using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class PedidosPresentacion : IPedidosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public PedidosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Pedidos Actualizar(Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Pedidos>(
                "Pedidos/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Pedidos Borrar(Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Pedidos>(
                "Pedidos/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Pedidos> Buscar(Pedidos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Pedidos>>(
                "Pedidos/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Pedidos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Pedidos>>(
                "Pedidos/Consultar",
                new Pedidos());
            task.Wait();
            return task.Result;
        }

        public Pedidos Insertar(Pedidos entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Pedidos>(
                "Pedidos/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Pedidos entidad)
        {
            return string.IsNullOrEmpty(entidad.Codigo) ||
                entidad.Proveedor == 0 ||
                entidad.Empleado == 0 ||
                entidad.Fecha == null ||
                string.IsNullOrEmpty(entidad.Estado);
        }
    }
}