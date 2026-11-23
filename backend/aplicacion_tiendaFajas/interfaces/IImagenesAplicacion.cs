using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IImagenesAplicacion
    {
        Imagenes Insertar(Imagenes entidad);
        Imagenes Actualizar(Imagenes entidad);
        List<Imagenes> Consultar();
        List<Imagenes> Buscar(Imagenes entidad);
        Imagenes Borrar(Imagenes entidad);

        /// <summary>
        /// Trae una imagen con su binario, para servir el archivo. Es la unica
        /// operacion que carga la columna Contenido.
        /// </summary>
        Imagenes? Obtener(int id);

        /// <summary>
        /// Fichas de las imagenes de un producto, sin el binario.
        /// </summary>
        List<Imagenes> DeProducto(int producto);
    }
}
