using aplicacion_tiendaFajas.entidades;

namespace presentacion_tiendaFajas.interfaces
{
    /// <summary>
    /// Segundo canal hacia el servicio, para lo que no puede viajar en JSON.
    ///
    /// IComunicaciones serializa a JSON y pasa la respuesta por una cadena de
    /// reemplazos de texto. Un binario no sobrevive ese camino, asi que los
    /// archivos van por multipart/form-data, que es el formato que el
    /// navegador ya produce para un input de tipo file.
    /// </summary>
    public interface IArchivos
    {
        /// <summary>
        /// Sube una imagen y devuelve su ficha, ya con el Id que asigno la base.
        /// </summary>
        Imagenes Subir(Stream contenido, string nombre, string tipo, int producto);

        /// <summary>
        /// Direccion publica de una imagen, para el atributo src de una
        /// etiqueta img. La sirve el servicio, no el front.
        /// </summary>
        string Url(int id);
    }
}
