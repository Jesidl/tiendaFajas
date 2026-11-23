using aplicacion_tiendaFajas.nucleo;
using pruebas_tiendaFajas.Nucleo;
using System.Text;

namespace pruebas_tiendaFajas.Dominio
{
    /// <summary>
    /// Pruebas del reconocimiento de imagenes.
    ///
    /// La regla es que el tipo de un archivo se deduce de sus primeros bytes y
    /// nunca del Content-Type que declara el cliente. Esta clase fija las dos
    /// razones de esa decision.
    ///
    /// La de fragilidad: un cliente HTTP cualquiera manda
    /// "application/octet-stream" para todo, y un PNG valido seria rechazado.
    /// Se descubrio al probar la subida desde PowerShell, que hace exactamente
    /// eso.
    ///
    /// La de seguridad: el Content-Type lo escribe quien sube el archivo. Si
    /// se guardara tal cual, el endpoint que sirve la imagen lo devolveria al
    /// navegador como cabecera, y se podria subir un HTML diciendo que es una
    /// imagen para que se sirviera como HTML desde este dominio.
    /// </summary>
    [TestClass]
    public class ImagenPrueba
    {
        [TestMethod]
        public void ReconoceUnPngPorSuFirma()
        {
            Assert.AreEqual("image/png", Imagen.Tipo(EntidadesNucleo.Png),
                "no reconocio un PNG valido");
        }

        [TestMethod]
        public void ReconoceLosCuatroFormatosQueSeAceptan()
        {
            var jpeg = Relleno(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
            var gif = Relleno(Encoding.ASCII.GetBytes("GIF89a"));

            // WEBP: "RIFF" + 4 bytes de tamaño + "WEBP"
            var webp = new List<byte>();
            webp.AddRange(Encoding.ASCII.GetBytes("RIFF"));
            webp.AddRange(new byte[] { 0x20, 0x00, 0x00, 0x00 });
            webp.AddRange(Encoding.ASCII.GetBytes("WEBPVP8 "));

            Assert.AreEqual("image/jpeg", Imagen.Tipo(jpeg), "no reconocio un JPEG");
            Assert.AreEqual("image/gif", Imagen.Tipo(gif), "no reconocio un GIF");
            Assert.AreEqual("image/webp", Imagen.Tipo(webp.ToArray()), "no reconocio un WEBP");
        }

        [TestMethod]
        public void RechazaLoQueNoEsImagen()
        {
            var texto = Relleno(Encoding.UTF8.GetBytes("esto no es una imagen"));
            var html = Relleno(Encoding.UTF8.GetBytes("<html><script>alert(1)</script>"));
            var pdf = Relleno(Encoding.ASCII.GetBytes("%PDF-1.7"));
            var ejecutable = Relleno(new byte[] { 0x4D, 0x5A });   // MZ, un .exe

            Assert.IsNull(Imagen.Tipo(texto), "acepto un archivo de texto");
            Assert.IsNull(Imagen.Tipo(html),
                "acepto un HTML: servirlo desde este dominio como si fuera una " +
                "imagen es justamente el riesgo que esta regla evita");
            Assert.IsNull(Imagen.Tipo(pdf), "acepto un PDF");
            Assert.IsNull(Imagen.Tipo(ejecutable), "acepto un ejecutable");
        }

        [TestMethod]
        public void RechazaLoVacioYLoDemasiadoCorto()
        {
            Assert.IsNull(Imagen.Tipo(null), "acepto null");
            Assert.IsNull(Imagen.Tipo(Array.Empty<byte>()), "acepto un arreglo vacio");

            // los primeros bytes de un PNG, pero el archivo se corta ahi: no
            // alcanza para decidir, y lo que no se puede reconocer se rechaza
            Assert.IsNull(Imagen.Tipo(new byte[] { 0x89, 0x50, 0x4E }),
                "acepto un archivo truncado");
        }

        [TestMethod]
        public void UnHtmlDisfrazadoDePngSigueSiendoHtml()
        {
            // el caso que motiva la regla: el cliente puede decir lo que
            // quiera, pero los bytes son los bytes
            var disfrazado = Relleno(Encoding.UTF8.GetBytes(
                "<html><body>no soy una imagen</body></html>"));

            Assert.IsNull(Imagen.Tipo(disfrazado),
                "el contenido manda sobre lo que declare el cliente");
        }

        /// <summary>Completa hasta 16 bytes, el minimo que Tipo necesita leer.</summary>
        private static byte[] Relleno(byte[] inicio)
        {
            var bytes = new byte[Math.Max(16, inicio.Length)];
            Array.Copy(inicio, bytes, inicio.Length);
            return bytes;
        }
    }
}
