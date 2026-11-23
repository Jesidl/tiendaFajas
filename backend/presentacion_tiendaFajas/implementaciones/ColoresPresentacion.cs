using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class ColoresPresentacion : IColoresPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ColoresPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Colores Actualizar(Colores entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Colores>(
                "Colores/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Colores Borrar(Colores entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Colores>(
                "Colores/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Colores> Buscar(Colores entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Colores>>(
                "Colores/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Colores> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Colores>>(
                "Colores/Consultar",
                new Colores());
            task.Wait();
            return task.Result;
        }

        public Colores Insertar(Colores entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Colores>(
                "Colores/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Colores entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Codigo_Hex);
        }
    }
}