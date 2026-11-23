using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class VariantesModel : PageModel
    {
        private IVariantesPresentacion IVariantesPresentacion;
        private IProductosPresentacion IProductosPresentacion;
        private ITallasPresentacion ITallasPresentacion;
        private IColoresPresentacion IColoresPresentacion;

        public VariantesModel(IVariantesPresentacion IVariantesPresentacion,
            IProductosPresentacion IProductosPresentacion,
            ITallasPresentacion ITallasPresentacion,
            IColoresPresentacion IColoresPresentacion)
        {
            this.IVariantesPresentacion = IVariantesPresentacion;
            this.IProductosPresentacion = IProductosPresentacion;
            this.ITallasPresentacion = ITallasPresentacion;
            this.IColoresPresentacion = IColoresPresentacion;
            this.Filtro = new Variantes();
        }

        [BindProperty] public List<Variantes> Lista { get; set; } = new();
        [BindProperty] public List<Productos> Productos { get; set; } = new();
        [BindProperty] public List<Tallas> Tallas { get; set; } = new();
        [BindProperty] public List<Colores> Colores { get; set; } = new();
        [BindProperty] public Variantes? Filtro { get; set; }
        [BindProperty] public Variantes? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Sku = this.Filtro!.Sku ?? "";
                Lista = this.IVariantesPresentacion.Buscar(this.Filtro!);
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
                this.Actual = new Variantes();
                this.Productos = this.IProductosPresentacion.Buscar(new Productos { Nombre = "" });
                this.Tallas = this.ITallasPresentacion.Buscar(new Tallas { Nombre = "" });
                this.Colores = this.IColoresPresentacion.Buscar(new Colores { Nombre = "" });
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
                    this.Actual = this.IVariantesPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IVariantesPresentacion.Actualizar(this.Actual!);
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
                this.Productos = this.IProductosPresentacion.Buscar(new Productos { Nombre = "" });
                this.Tallas = this.ITallasPresentacion.Buscar(new Tallas { Nombre = "" });
                this.Colores = this.IColoresPresentacion.Buscar(new Colores { Nombre = "" });
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
                this.Actual = this.IVariantesPresentacion.Borrar(this.Actual!);
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