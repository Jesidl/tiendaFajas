using aplicacion_tiendaFajas.entidades;
using pruebas_tiendaFajas.Nucleo;
using System.Text.Json;

namespace pruebas_tiendaFajas.Comunicaciones
{
    /// <summary>
    /// Pruebas de que el binario de una imagen nunca entra al canal JSON.
    ///
    /// Es la regla que sostiene todo el diseño de Imagenes. El cliente HTTP
    /// pasa cada respuesta del servicio por una cadena de reemplazos de texto
    /// que, entre otras cosas, cambia "null" por una cadena vacia. Una imagen
    /// en base64 son miles de caracteres del alfabeto A-Za-z0-9+/, asi que que
    /// contenga esa subcadena en algun punto es practicamente seguro: llegaria
    /// corrupta y la imagen no abriria, con un error imposible de reproducir
    /// con un archivo distinto.
    ///
    /// Por eso la columna Contenido lleva [JsonIgnore] y el archivo viaja por
    /// un endpoint propio con multipart/form-data. Si alguien quitara ese
    /// atributo, estas pruebas lo avisan.
    /// </summary>
    [TestClass]
    public class BinarioFueraDelJsonPrueba
    {
        private static Imagenes Ficha()
        {
            return new Imagenes
            {
                Id = 7,
                Producto = 3,
                Nombre = "faja.png",
                Tipo = "image/png",
                Tamano = EntidadesNucleo.Png.Length,
                Fecha = new DateTime(2026, 11, 22),
                Contenido = EntidadesNucleo.Png
            };
        }

        [TestMethod]
        public void LaRespuestaDelServicioNoLlevaElBinario()
        {
            // el servicio serializa con System.Text.Json
            var json = JsonSerializer.Serialize(Ficha());

            Assert.DoesNotContain("Contenido", json,
                "la columna Contenido esta saliendo en el JSON del servicio: " +
                "una imagen en base64 no sobrevive la cadena de Replace del " +
                "cliente HTTP");

            // y tampoco los bytes por otra via
            Assert.DoesNotContain(Convert.ToBase64String(EntidadesNucleo.Png), json,
                "los bytes del archivo estan viajando en el JSON");

            // lo que si debe viajar es la ficha
            Assert.Contains("faja.png", json, "se perdio el nombre del archivo");
            Assert.Contains("image/png", json, "se perdio el tipo del archivo");
            Assert.Contains("Tamano", json, "se perdio el tamaño del archivo");
        }

        [TestMethod]
        public void LoQueLlegaDelFrontNoPuedeTraerBinario()
        {
            // [JsonIgnore] tambien aplica al deserializar: aunque alguien
            // construyera a mano una peticion con la columna Contenido, el
            // servicio la descarta. Una entidad no puede traer un archivo por
            // el canal equivocado.
            var conBinario =
                "{\"Id\":0,\"Producto\":3,\"Nombre\":\"faja.png\"," +
                "\"Tipo\":\"image/png\",\"Tamano\":70," +
                "\"Contenido\":\"" + Convert.ToBase64String(EntidadesNucleo.Png) + "\"}";

            var entidad = JsonSerializer.Deserialize<Imagenes>(conBinario);

            Assert.IsNotNull(entidad, "no se pudo deserializar la ficha");
            Assert.IsNull(entidad!.Contenido,
                "el servicio acepto un binario por el canal JSON");
            Assert.AreEqual("faja.png", entidad.Nombre,
                "se perdio el resto de la ficha");
        }

        [TestMethod]
        public void LaFichaSobreviveLaCadenaDeReemplazos()
        {
            // lo que si pasa por el conversor es la ficha, y tiene que llegar
            // completa: de ahi sale el src de la etiqueta img
            var metodo = typeof(presentacion_tiendaFajas.implementaciones.Comunicaciones)
                .GetMethod("Replace", System.Reflection.BindingFlags.Instance |
                                      System.Reflection.BindingFlags.NonPublic);

            Assert.IsNotNull(metodo, "Comunicaciones ya no tiene el metodo Replace");

            var objeto = new presentacion_tiendaFajas.implementaciones.Comunicaciones();
            var entrada = JsonSerializer.Serialize(Ficha());
            var convertida = (string)metodo!.Invoke(objeto, new object[] { entrada })!;

            var vuelta = Newtonsoft.Json.JsonConvert.DeserializeObject<Imagenes>(convertida);

            Assert.IsNotNull(vuelta, "la ficha convertida no se pudo deserializar");
            Assert.AreEqual(7, vuelta!.Id, "se perdio el Id, que es el que forma la URL");
            Assert.AreEqual("faja.png", vuelta.Nombre, "se perdio el nombre");
            Assert.AreEqual("image/png", vuelta.Tipo, "se perdio el tipo");
        }
    }
}
