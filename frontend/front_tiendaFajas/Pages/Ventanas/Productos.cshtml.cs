using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
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

        public ProductosModel(IProductosPresentacion IProductosPresentacion,
            ICategoriasPresentacion ICategoriasPresentacion,
            IMarcasPresentacion IMarcasPresentacion,
            IMaterialesPresentacion IMaterialesPresentacion)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.ICategoriasPresentacion = ICategoriasPresentacion;
            this.IMarcasPresentacion = IMarcasPresentacion;
            this.IMaterialesPresentacion = IMaterialesPresentacion;
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
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
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