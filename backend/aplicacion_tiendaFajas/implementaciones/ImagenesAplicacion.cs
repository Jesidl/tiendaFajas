using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class ImagenesAplicacion : IImagenesAplicacion
    {
        private IConexion IConexion;

        public ImagenesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Imagenes Insertar(Imagenes entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            if (entidad.Contenido == null || entidad.Contenido.Length == 0)
                throw new Exception("Falta el archivo");

            if (entidad.Contenido.Length > Imagen.Maximo)
                throw new Exception("El archivo pasa de 4 MB");

            // El tipo se deduce de los bytes del archivo y se sobreescribe lo
            // que haya declarado el cliente. La regla vive aqui y no en el
            // controlador para que valga por cualquier via de entrada,
            // incluidas las pruebas.
            var tipo = Imagen.Tipo(entidad.Contenido);
            if (tipo == null)
                throw new Exception("Solo se aceptan imagenes PNG, JPEG, WEBP o GIF");

            entidad.Tipo = tipo;
            entidad.Tamano = entidad.Contenido.Length;
            entidad.Fecha = DateTime.Now;

            this.IConexion.Imagenes!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        /// <summary>
        /// Solo actualiza la ficha, nunca el binario.
        ///
        /// No se puede marcar la entidad entera como Modified, que es lo que
        /// hace el resto de la aplicacion: la entidad llega desde el front sin
        /// la columna Contenido, porque esa columna no viaja en JSON, y marcar
        /// todo como modificado escribiria null encima del archivo. Se marcan
        /// a mano las columnas que si se pueden cambiar.
        /// </summary>
        public Imagenes Actualizar(Imagenes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Imagenes>(entidad);
            entry.State = EntityState.Unchanged;
            entry.Property(x => x.Nombre).IsModified = true;
            entry.Property(x => x.Producto).IsModified = true;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Imagenes Borrar(Imagenes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Imagenes!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Imagenes> Buscar(Imagenes entidad)
        {
            return Ficha(this.IConexion.Imagenes!
                .Where(x => x.Nombre!.Contains(entidad.Nombre!)))
                .ToList();
        }

        public List<Imagenes> Consultar()
        {
            return Ficha(this.IConexion.Imagenes!).Take(5).ToList();
        }

        public List<Imagenes> DeProducto(int producto)
        {
            return Ficha(this.IConexion.Imagenes!
                .Where(x => x.Producto == producto))
                .ToList();
        }

        public Imagenes? Obtener(int id)
        {
            // la unica consulta que trae el binario
            return this.IConexion.Imagenes!
                .FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Proyeccion sin la columna Contenido.
        ///
        /// Con Include() el binario viajaria de SQL Server a memoria en cada
        /// listado, aunque el JSON luego lo descarte: megabytes leidos para
        /// nada. Proyectar deja la columna fuera del SELECT.
        /// </summary>
        private static IQueryable<Imagenes> Ficha(IQueryable<Imagenes> consulta)
        {
            return consulta.Select(x => new Imagenes
            {
                Id = x.Id,
                Producto = x.Producto,
                Nombre = x.Nombre,
                Tipo = x.Tipo,
                Tamano = x.Tamano,
                Fecha = x.Fecha,
                _Producto = x._Producto
            });
        }

        /// <summary>
        /// El Tipo no se valida: no lo suministra el cliente, lo deduce
        /// Insertar de los bytes del archivo.
        /// </summary>
        private bool Validar(Imagenes entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                entidad.Producto == 0;
        }
    }
}
