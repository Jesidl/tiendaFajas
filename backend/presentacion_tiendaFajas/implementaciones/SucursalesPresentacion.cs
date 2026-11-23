using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class SucursalesPresentacion : ISucursalesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public SucursalesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Sucursales Actualizar(Sucursales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Sucursales>(
                "Sucursales/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Sucursales Borrar(Sucursales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Sucursales>(
                "Sucursales/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Sucursales> Buscar(Sucursales entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Sucursales>>(
                "Sucursales/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Sucursales> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Sucursales>>(
                "Sucursales/Consultar",
                new Sucursales());
            task.Wait();
            return task.Result;
        }

        public Sucursales Insertar(Sucursales entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Sucursales>(
                "Sucursales/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Sucursales entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Direccion) ||
                string.IsNullOrEmpty(entidad.Ciudad) ||
                string.IsNullOrEmpty(entidad.Telefono);
        }
    }
}