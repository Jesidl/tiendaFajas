using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class ProductosModel : PageModel
    {
        private IProductosPresentacion IProductosPresentacion;
        private ICategoriasPresentacion ICategoriasPresentacion;
        private IMarcasPresentacion IMarcasPresentacion;
        private IMaterialesPresentacion IMaterialesPresentacion;
        private IImagenesPresentacion IImagenesPresentacion;
        private IArchivos IArchivos;

        public ProductosModel(IProductosPresentacion IProductosPresentacion,
            ICategoriasPresentacion ICategoriasPresentacion,
            IMarcasPresentacion IMarcasPresentacion,
            IMaterialesPresentacion IMaterialesPresentacion,
            IImagenesPresentacion IImagenesPresentacion,
            IArchivos IArchivos)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.ICategoriasPresentacion = ICategoriasPresentacion;
            this.IMarcasPresentacion = IMarcasPresentacion;
            this.IMaterialesPresentacion = IMaterialesPresentacion;
            this.IImagenesPresentacion = IImagenesPresentacion;
            this.IArchivos = IArchivos;
            this.Filtro = new Productos();
        }

        [BindProperty] public List<Productos> Lista { get; set; } = new();
        [BindProperty] public List<Categorias> Categorias { get; set; } = new();
        [BindProperty] public List<Marcas> Marcas { get; set; } = new();
        [BindProperty] public List<Materiales> Materiales { get; set; } = new();
        [BindProperty] public Productos? Filtro { get; set; }
        [BindProperty] public Productos? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        /// <summary>Fichas de las imagenes del producto que se esta editando.</summary>
        public List<Imagenes> Imagenes { get; set; } = new();

        /// <summary>
        /// El archivo que llega del input de tipo file.
        ///
        /// Lleva [BindProperty] como el resto: sin ese atributo la propiedad
        /// queda en null aunque el formulario mande el archivo y tenga el
        /// enctype correcto, y la subida falla sin ningun error visible.
        /// </summary>
        [BindProperty] public IFormFile? Archivo { get; set; }

        /// <summary>
        /// Direccion de la que el navegador descarga una imagen. La sirve el
        /// servicio, no el front: el binario nunca pasa por este proceso.
        /// </summary>
        public string UrlImagen(int id)
        {
            return this.IArchivos.Url(id);
        }

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Nombre = this.Filtro!.Nombre ?? "";
                Lista = this.IProductosPresentacion.Buscar(this.Filtro!);
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtNuevo()
        {
            try
            {
                if (!Sesion()) return;

                Formulario = true;
                this.Actual = new Productos();
                this.Categorias = this.ICategoriasPresentacion.Buscar(new Categorias { Nombre = "" });
                this.Marcas = this.IMarcasPresentacion.Buscar(new Marcas { Nombre = "" });
                this.Materiales = this.IMaterialesPresentacion.Buscar(new Materiales { Nombre = "" });
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtGuardar()
        {
            try
            {
                if (!Sesion()) return;

                if (this.Actual!.Id == 0)
                    this.Actual = this.IProductosPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IProductosPresentacion.Actualizar(this.Actual!);
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
            finally
            {
                OnPostBtBuscar();
            }
        }

        public void OnPostBtEditar(string data)
        {
            try
            {
                if (!Sesion()) return;

                Formulario = true;
                OnPostBtBuscar();
                Actual = this.Lista.FirstOrDefault(x => x.Id == Convert.ToInt32(data));
                this.Categorias = this.ICategoriasPresentacion.Buscar(new Categorias { Nombre = "" });
                this.Marcas = this.IMarcasPresentacion.Buscar(new Marcas { Nombre = "" });
                this.Materiales = this.IMaterialesPresentacion.Buscar(new Materiales { Nombre = "" });
                CargarImagenes();
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtSubir()
        {
            try
            {
                if (!Sesion()) return;

                Formulario = true;

                // una imagen necesita a que producto pertenecer, y eso exige
                // que el producto ya tenga Id
                if (this.Actual == null || this.Actual.Id == 0)
                {
                    ViewData["Mensaje"] = "Guarde el producto antes de subirle una imagen";
                    return;
                }

                if (this.Archivo == null || this.Archivo.Length == 0)
                {
                    ViewData["Mensaje"] = "Escoja un archivo";
                    return;
                }

                using var contenido = this.Archivo.OpenReadStream();
                this.IArchivos.Subir(
                    contenido,
                    Path.GetFileName(this.Archivo.FileName),
                    this.Archivo.ContentType,
                    this.Actual.Id);
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
            finally
            {
                Recargar();
            }
        }

        public void OnPostBtBorrarImagen(string data)
        {
            try
            {
                if (!Sesion()) return;

                Formulario = true;
                this.IImagenesPresentacion.Borrar(
                    new Imagenes { Id = Convert.ToInt32(data) });
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
            finally
            {
                Recargar();
            }
        }

        /// <summary>
        /// Vuelve a dejar el formulario como estaba, con sus desplegables y su
        /// galeria, despues de subir o borrar una imagen.
        /// </summary>
        private void Recargar()
        {
            try
            {
                if (this.Actual == null || this.Actual.Id == 0)
                    return;

                this.Categorias = this.ICategoriasPresentacion.Buscar(new Categorias { Nombre = "" });
                this.Marcas = this.IMarcasPresentacion.Buscar(new Marcas { Nombre = "" });
                this.Materiales = this.IMaterialesPresentacion.Buscar(new Materiales { Nombre = "" });
                CargarImagenes();
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        private void CargarImagenes()
        {
            if (this.Actual == null || this.Actual.Id == 0)
                return;

            this.Imagenes = this.IImagenesPresentacion.DeProducto(this.Actual.Id);
        }

        public void OnPostBtPregBorrar(string data)
        {
            try
            {
                if (!Sesion()) return;

                Borrar = true;
                OnPostBtBuscar();
                Actual = this.Lista.FirstOrDefault(x => x.Id == Convert.ToInt32(data));
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtBorrar()
        {
            try
            {
                if (!Sesion()) return;

                if (this.Actual!.Id == 0)
                    return;
                this.Actual = this.IProductosPresentacion.Borrar(this.Actual!);
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
            finally
            {
                OnPostBtBuscar();
            }
        }

        private bool Sesion()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (String.IsNullOrEmpty(usuario))
            {
                HttpContext.Response.Redirect("/");
                return false;
            }

            // viaja al servicio en la cabecera X-Usuario, para la auditoria
            SesionActual.Usuario = usuario;
            return true;
        }
    }
}