using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class TallasPresentacion : ITallasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public TallasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Tallas Actualizar(Tallas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Tallas>(
                "Tallas/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Tallas Borrar(Tallas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Tallas>(
                "Tallas/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Tallas> Buscar(Tallas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Tallas>>(
                "Tallas/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Tallas> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Tallas>>(
                "Tallas/Consultar",
                new Tallas());
            task.Wait();
            return task.Result;
        }

        public Tallas Insertar(Tallas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Tallas>(
                "Tallas/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Tallas entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}