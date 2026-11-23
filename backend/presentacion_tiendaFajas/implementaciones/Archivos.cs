using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.nucleo;
using Newtonsoft.Json;
using presentacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.nucleo;

namespace presentacion_tiendaFajas.implementaciones
{
    public class Archivos : IArchivos
    {
        public Imagenes Subir(Stream contenido, string nombre, string tipo, int producto)
        {
            if (contenido == null)
                throw new Exception("Falta el archivo");

            if (producto == 0)
                throw new Exception("Falta informacion");

            using var cliente = new HttpClient();
            cliente.Timeout = new TimeSpan(0, 4, 0);

            var usuario = SesionActual.Usuario;
            if (!string.IsNullOrEmpty(usuario))
                cliente.DefaultRequestHeaders.Add(ContextoUsuario.Cabecera, usuario);

            using var formulario = new MultipartFormDataContent();
            var archivo = new StreamContent(contenido);
            archivo.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(tipo);

            // los nombres "archivo" y "producto" tienen que coincidir con los
            // parametros [FromForm] de ImagenesController.Subir
            formulario.Add(archivo, "archivo", nombre);
            formulario.Add(new StringContent(producto.ToString()), "producto");

            var respuesta = cliente
                .PostAsync(Servidor.Ruta("Imagenes/Subir"), formulario)
                .Result;

            if (!respuesta.IsSuccessStatusCode)
                throw new Exception("Error Comunicacion");

            // La respuesta es la ficha de la imagen, sin el binario, asi que
            // se deserializa directo. No pasa por el Replace de Comunicaciones
            // porque no hace falta: aqui el JSON llega tal como lo genero el
            // servicio, sin el doble escapado que produce el otro canal.
            var texto = respuesta.Content.ReadAsStringAsync().Result;
            var ficha = JsonConvert.DeserializeObject<Imagenes>(texto);

            if (ficha == null)
                throw new Exception("El servicio no devolvio la imagen");

            return ficha;
        }

        public string Url(int id)
        {
            return Servidor.Ruta("Imagenes/Ver/" + id);
        }
    }
}
