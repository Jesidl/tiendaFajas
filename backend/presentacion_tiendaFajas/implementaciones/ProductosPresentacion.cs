using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class ProductosPresentacion : IProductosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ProductosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Productos Actualizar(Productos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Productos>(
                "Productos/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Productos Borrar(Productos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Productos>(
                "Productos/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Productos> Buscar(Productos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Productos>>(
                "Productos/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Productos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Productos>>(
                "Productos/Consultar",
                new Productos());
            task.Wait();
            return task.Result;
        }

        public Productos Insertar(Productos entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Productos>(
                "Productos/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Productos entidad)
        {
            return string.IsNullOrEmpty(entidad.Codigo) ||
                string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                entidad.Categoria == 0 ||
                entidad.Marca == 0 ||
                entidad.Material == 0;
        }
    }
}