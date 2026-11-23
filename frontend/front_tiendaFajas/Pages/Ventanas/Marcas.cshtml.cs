using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class MarcasModel : PageModel
    {
        private IMarcasPresentacion IMarcasPresentacion;

        public MarcasModel(IMarcasPresentacion IMarcasPresentacion)
        {
            this.IMarcasPresentacion = IMarcasPresentacion;
            this.Filtro = new Marcas();
        }

        [BindProperty] public List<Marcas> Lista { get; set; } = new();
        [BindProperty] public Marcas? Filtro { get; set; }
        [BindProperty] public Marcas? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Nombre = this.Filtro!.Nombre ?? "";
                Lista = this.IMarcasPresentacion.Buscar(this.Filtro!);
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
                this.Actual = new Marcas();
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
                    this.Actual = this.IMarcasPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IMarcasPresentacion.Actualizar(this.Actual!);
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
                this.Actual = this.IMarcasPresentacion.Borrar(this.Actual!);
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
