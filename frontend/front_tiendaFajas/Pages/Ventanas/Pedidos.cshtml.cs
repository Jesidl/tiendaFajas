using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class PedidosModel : PageModel
    {
        private IPedidosPresentacion IPedidosPresentacion;
        private IProveedoresPresentacion IProveedoresPresentacion;
        private IEmpleadosPresentacion IEmpleadosPresentacion;

        public PedidosModel(IPedidosPresentacion IPedidosPresentacion,
            IProveedoresPresentacion IProveedoresPresentacion,
            IEmpleadosPresentacion IEmpleadosPresentacion)
        {
            this.IPedidosPresentacion = IPedidosPresentacion;
            this.IProveedoresPresentacion = IProveedoresPresentacion;
            this.IEmpleadosPresentacion = IEmpleadosPresentacion;
            this.Filtro = new Pedidos();
        }

        [BindProperty] public List<Pedidos> Lista { get; set; } = new();
        [BindProperty] public List<Proveedores> Proveedores { get; set; } = new();
        [BindProperty] public List<Empleados> Empleados { get; set; } = new();
        [BindProperty] public Pedidos? Filtro { get; set; }
        [BindProperty] public Pedidos? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Codigo = this.Filtro!.Codigo ?? "";
                Lista = this.IPedidosPresentacion.Buscar(this.Filtro!);
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
                this.Actual = new Pedidos();
                this.Proveedores = this.IProveedoresPresentacion.Buscar(new Proveedores { Nombre_Empresa = "" });
                this.Empleados = this.IEmpleadosPresentacion.Buscar(new Empleados { Nombre = "" });
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
                    this.Actual = this.IPedidosPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IPedidosPresentacion.Actualizar(this.Actual!);
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
                this.Proveedores = this.IProveedoresPresentacion.Buscar(new Proveedores { Nombre_Empresa = "" });
                this.Empleados = this.IEmpleadosPresentacion.Buscar(new Empleados { Nombre = "" });
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
                this.Actual = this.IPedidosPresentacion.Borrar(this.Actual!);
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