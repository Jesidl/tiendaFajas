using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace servicio_tiendaFajas.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImagenesController : ControllerBase
    {
        private IImagenesAplicacion IImagenesAplicacion;

        public ImagenesController(IImagenesAplicacion IImagenesAplicacion)
        {
            this.IImagenesAplicacion = IImagenesAplicacion;
        }

        [HttpPost]
        public List<Imagenes> Consultar()
        {
            return this.IImagenesAplicacion.Consultar();
        }

        [HttpPost]
        public List<Imagenes> Buscar(Imagenes entidad)
        {
            return this.IImagenesAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Imagenes Insertar(Imagenes entidad)
        {
            return this.IImagenesAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Imagenes Actualizar(Imagenes entidad)
        {
            return this.IImagenesAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Imagenes Borrar(Imagenes entidad)
        {
            return this.IImagenesAplicacion.Borrar(entidad);
        }

        [HttpPost]
        public List<Imagenes> DeProducto(Imagenes entidad)
        {
            return this.IImagenesAplicacion.DeProducto(entidad.Producto);
        }

        // ------------------------------------------------------------------
        // Los dos endpoints que no siguen la convencion de POST con JSON, y
        // por que.
        //
        // El binario de una imagen no puede viajar en el JSON: el cliente HTTP
        // pasa toda respuesta por una cadena de reemplazos de texto que
        // sustituye "null" por una cadena vacia, y una imagen en base64
        // contiene esa subcadena casi con seguridad. Ademas base64 infla el
        // tamaño un 33 por ciento.
        //
        // Subir usa multipart/form-data, que es el formato que el navegador ya
        // produce para un <input type="file">. Ver usa GET porque lo consume
        // la etiqueta <img> del navegador, que solo sabe hacer GET.
        // ------------------------------------------------------------------

        [HttpPost]
        public Imagenes Subir([FromForm] IFormFile archivo, [FromForm] int producto)
        {
            if (archivo == null || archivo.Length == 0)
                throw new Exception("Falta el archivo");

            using var memoria = new MemoryStream();
            archivo.CopyTo(memoria);

            // El controlador solo lee el archivo. El tamaño y el tipo los
            // decide la capa de aplicacion a partir de los bytes: el
            // Content-Type que manda el cliente no se usa, porque lo escribe
            // quien sube el archivo y por tanto no prueba nada.
            return this.IImagenesAplicacion.Insertar(new Imagenes
            {
                Producto = producto,
                Nombre = Path.GetFileName(archivo.FileName),
                Contenido = memoria.ToArray()
            });
        }

        [HttpGet("{id:int}")]
        public IActionResult Ver(int id)
        {
            var imagen = this.IImagenesAplicacion.Obtener(id);

            if (imagen == null || imagen.Contenido == null)
                return NotFound();

            // el navegador puede cachearla: el contenido de un Id no cambia,
            // porque Actualizar solo toca la ficha y nunca el binario
            Response.Headers.CacheControl = "public, max-age=86400";

            // el navegador respeta el tipo que declaramos en vez de adivinarlo
            // mirando el contenido. El tipo guardado proviene de la firma de
            // bytes del archivo, asi que es el correcto, y dejar que el
            // navegador lo reinterprete solo abriria la puerta a que trate
            // como otra cosa algo que para nosotros es una imagen.
            Response.Headers.XContentTypeOptions = "nosniff";

            return File(imagen.Contenido, imagen.Tipo ?? "application/octet-stream");
        }
    }
}
