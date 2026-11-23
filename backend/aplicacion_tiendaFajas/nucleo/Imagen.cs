namespace aplicacion_tiendaFajas.nucleo
{
    /// <summary>
    /// Reglas sobre que archivo se acepta como imagen.
    ///
    /// El tipo se deduce de los primeros bytes del archivo y nunca del
    /// Content-Type que declara el cliente, por dos razones.
    ///
    /// La primera es fragilidad: un navegador manda "image/png" para un PNG,
    /// pero un cliente HTTP cualquiera manda "application/octet-stream" para
    /// todo, y el archivo seria rechazado aunque sea valido.
    ///
    /// La segunda es seguridad: el Content-Type lo escribe quien sube el
    /// archivo, asi que es una afirmacion sin respaldo. Si se guardara tal
    /// cual, el endpoint que sirve la imagen lo devolveria al navegador como
    /// cabecera, y alguien podria subir un HTML diciendo que es una imagen
    /// para que luego se sirviera como HTML desde este dominio.
    ///
    /// La firma de bytes, en cambio, es el archivo mismo: no se puede
    /// declarar, solo ser.
    /// </summary>
    public class Imagen
    {
        /// <summary>4 MB. Una fotografia de producto no necesita mas.</summary>
        public const long Maximo = 4 * 1024 * 1024;

        /// <summary>
        /// Devuelve el tipo MIME del archivo, o null si no es ninguna de las
        /// imagenes que se aceptan. Lista blanca, no lista negra: lo que no
        /// esta reconocido se rechaza.
        /// </summary>
        public static string? Tipo(byte[]? bytes)
        {
            if (bytes == null || bytes.Length < 12)
                return null;

            // PNG: 89 'P' 'N' 'G' CR LF 1A LF
            if (Coincide(bytes, 0, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A))
                return "image/png";

            // JPEG: FF D8 FF
            if (Coincide(bytes, 0, 0xFF, 0xD8, 0xFF))
                return "image/jpeg";

            // GIF: "GIF8"
            if (Coincide(bytes, 0, 0x47, 0x49, 0x46, 0x38))
                return "image/gif";

            // WEBP: "RIFF" ... "WEBP", con el tamaño en medio
            if (Coincide(bytes, 0, 0x52, 0x49, 0x46, 0x46) &&
                Coincide(bytes, 8, 0x57, 0x45, 0x42, 0x50))
                return "image/webp";

            return null;
        }

        private static bool Coincide(byte[] bytes, int desde, params byte[] firma)
        {
            if (bytes.Length < desde + firma.Length)
                return false;

            for (var i = 0; i < firma.Length; i++)
            {
                if (bytes[desde + i] != firma[i])
                    return false;
            }

            return true;
        }
    }
}
