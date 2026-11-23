using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class ImagenesPresentacion : IImagenesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ImagenesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Imagenes Actualizar(Imagenes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Imagenes>(
                "Imagenes/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Imagenes Borrar(Imagenes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Imagenes>(
                "Imagenes/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Imagenes> Buscar(Imagenes entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Imagenes>>(
                "Imagenes/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Imagenes> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Imagenes>>(
                "Imagenes/Consultar",
                new Imagenes());
            task.Wait();
            return task.Result;
        }

        public List<Imagenes> DeProducto(int producto)
        {
            var task = this.IComunicaciones.Ejecutar<List<Imagenes>>(
                "Imagenes/DeProducto",
                new Imagenes { Producto = producto });
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Insertar existe para completar el contrato de las cinco
        /// operaciones, pero el camino normal para crear una imagen es
        /// IArchivos.Subir: aqui no hay forma de mandar el binario, porque la
        /// columna Contenido no viaja en JSON.
        /// </summary>
        public Imagenes Insertar(Imagenes entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Imagenes>(
                "Imagenes/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Imagenes entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Tipo) ||
                entidad.Producto == 0;
        }
    }
}
