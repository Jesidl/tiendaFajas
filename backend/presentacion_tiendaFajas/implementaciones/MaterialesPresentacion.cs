using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class MaterialesPresentacion : IMaterialesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public MaterialesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Materiales Actualizar(Materiales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Materiales>(
                "Materiales/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Materiales Borrar(Materiales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Materiales>(
                "Materiales/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Materiales> Buscar(Materiales entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Materiales>>(
                "Materiales/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Materiales> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Materiales>>(
                "Materiales/Consultar",
                new Materiales());
            task.Wait();
            return task.Result;
        }

        public Materiales Insertar(Materiales entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Materiales>(
                "Materiales/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Materiales entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}