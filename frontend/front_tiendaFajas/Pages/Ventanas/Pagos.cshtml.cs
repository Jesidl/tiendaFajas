using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class PagosModel : PageModel
    {
        private IPagosPresentacion IPagosPresentacion;
        private IVentasPresentacion IVentasPresentacion;
        private IMetodos_PagoPresentacion IMetodos_PagoPresentacion;

        public PagosModel(IPagosPresentacion IPagosPresentacion,
            IVentasPresentacion IVentasPresentacion,
            IMetodos_PagoPresentacion IMetodos_PagoPresentacion)
        {
            this.IPagosPresentacion = IPagosPresentacion;
            this.IVentasPresentacion = IVentasPresentacion;
            this.IMetodos_PagoPresentacion = IMetodos_PagoPresentacion;
            this.Filtro = new Pagos();
        }

        [BindProperty] public List<Pagos> Lista { get; set; } = new();
        [BindProperty] public List<Ventas> Ventas { get; set; } = new();
        [BindProperty] public List<Metodos_Pago> Metodos_Pago { get; set; } = new();
        [BindProperty] public Pagos? Filtro { get; set; }
        [BindProperty] public Pagos? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Filtro = this.Filtro!.Filtro ?? "";
                Lista = this.IPagosPresentacion.Buscar(this.Filtro!);
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
                this.Actual = new Pagos();
                this.Ventas = this.IVentasPresentacion.Buscar(new Ventas { Codigo = "" });
                this.Metodos_Pago = this.IMetodos_PagoPresentacion.Buscar(new Metodos_Pago { Nombre = "" });
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
                    this.Actual = this.IPagosPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IPagosPresentacion.Actualizar(this.Actual!);
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
                this.Ventas = this.IVentasPresentacion.Buscar(new Ventas { Codigo = "" });
                this.Metodos_Pago = this.IMetodos_PagoPresentacion.Buscar(new Metodos_Pago { Nombre = "" });
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
                this.Actual = this.IPagosPresentacion.Borrar(this.Actual!);
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