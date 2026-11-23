using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IImagenesPresentacion
    {
        Imagenes Insertar(Imagenes entidad);
        Imagenes Actualizar(Imagenes entidad);
        List<Imagenes> Consultar();
        List<Imagenes> Buscar(Imagenes entidad);
        Imagenes Borrar(Imagenes entidad);

        /// <summary>Fichas de las imagenes de un producto, sin el binario.</summary>
        List<Imagenes> DeProducto(int producto);
    }
}
