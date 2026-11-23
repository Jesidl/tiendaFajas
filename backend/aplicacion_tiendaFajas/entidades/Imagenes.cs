using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace aplicacion_tiendaFajas.entidades
{
    /// <summary>
    /// Fotografia de un producto.
    ///
    /// Es una tabla aparte y no una columna en Productos por dos razones. La
    /// primera es que un producto puede tener varias fotos. La segunda, y mas
    /// importante, es que un VARBINARY(MAX) dentro de Productos viajaria en
    /// cada Consultar y en cada Buscar de productos, aunque nadie lo pida.
    /// </summary>
    public class Imagenes
    {
        public int Id { get; set; }
        public int Producto { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public long Tamano { get; set; }
        public DateTime? Fecha { get; set; }

        /// <summary>
        /// El binario del archivo.
        ///
        /// Se excluye de la serializacion JSON a proposito. El cliente HTTP
        /// pasa toda respuesta del servicio por una cadena de reemplazos de
        /// texto que, entre otras cosas, sustituye "null" por una cadena
        /// vacia. Una imagen en base64 son miles de caracteres del alfabeto
        /// A-Za-z0-9+/, asi que que contenga la subcadena "null" en algun
        /// punto es practicamente seguro, y llegaria corrupta.
        ///
        /// Por eso el binario no usa ese canal: sube y baja por un endpoint
        /// propio con multipart/form-data, sin pasar por JSON. Lo que viaja en
        /// JSON es solo la ficha: nombre, tipo, tamaño y a que producto
        /// pertenece.
        /// </summary>
        [JsonIgnore]
        public byte[]? Contenido { get; set; }

        [ForeignKey("Producto")] public Productos? _Producto { get; set; }
    }
}
