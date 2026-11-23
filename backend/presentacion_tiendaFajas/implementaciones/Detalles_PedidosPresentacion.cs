using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class Detalles_PedidosPresentacion : IDetalles_PedidosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public Detalles_PedidosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Detalles_Pedidos Actualizar(Detalles_Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Detalles_Pedidos>(
                "Detalles_Pedidos/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Detalles_Pedidos Borrar(Detalles_Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Detalles_Pedidos>(
                "Detalles_Pedidos/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Detalles_Pedidos> Buscar(Detalles_Pedidos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Detalles_Pedidos>>(
                "Detalles_Pedidos/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Detalles_Pedidos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Detalles_Pedidos>>(
                "Detalles_Pedidos/Consultar",
                new Detalles_Pedidos());
            task.Wait();
            return task.Result;
        }

        public Detalles_Pedidos Insertar(Detalles_Pedidos entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Detalles_Pedidos>(
                "Detalles_Pedidos/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Detalles_Pedidos entidad)
        {
            return entidad.Pedido == 0 ||
                entidad.Variante == 0;
        }
    }
}