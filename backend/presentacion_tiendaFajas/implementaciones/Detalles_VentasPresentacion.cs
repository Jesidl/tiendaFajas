using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class Detalles_VentasPresentacion : IDetalles_VentasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public Detalles_VentasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Detalles_Ventas Actualizar(Detalles_Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Detalles_Ventas>(
                "Detalles_Ventas/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Detalles_Ventas Borrar(Detalles_Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Detalles_Ventas>(
                "Detalles_Ventas/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Detalles_Ventas> Buscar(Detalles_Ventas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Detalles_Ventas>>(
                "Detalles_Ventas/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Detalles_Ventas> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Detalles_Ventas>>(
                "Detalles_Ventas/Consultar",
                new Detalles_Ventas());
            task.Wait();
            return task.Result;
        }

        public Detalles_Ventas Insertar(Detalles_Ventas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Detalles_Ventas>(
                "Detalles_Ventas/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Detalles_Ventas entidad)
        {
            return entidad.Venta == 0 ||
                entidad.Variante == 0;
        }
    }
}