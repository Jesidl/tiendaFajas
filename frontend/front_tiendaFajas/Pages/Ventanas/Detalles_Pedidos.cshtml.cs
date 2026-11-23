using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using presentacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages.Ventanas
{
    public class Detalles_PedidosModel : PageModel
    {
        private IDetalles_PedidosPresentacion IDetalles_PedidosPresentacion;
        private IPedidosPresentacion IPedidosPresentacion;
        private IVariantesPresentacion IVariantesPresentacion;

        public Detalles_PedidosModel(IDetalles_PedidosPresentacion IDetalles_PedidosPresentacion,
            IPedidosPresentacion IPedidosPresentacion,
            IVariantesPresentacion IVariantesPresentacion)
        {
            this.IDetalles_PedidosPresentacion = IDetalles_PedidosPresentacion;
            this.IPedidosPresentacion = IPedidosPresentacion;
            this.IVariantesPresentacion = IVariantesPresentacion;
            this.Filtro = new Detalles_Pedidos();
        }

        [BindProperty] public List<Detalles_Pedidos> Lista { get; set; } = new();
        [BindProperty] public List<Pedidos> Pedidos { get; set; } = new();
        [BindProperty] public List<Variantes> Variantes { get; set; } = new();
        [BindProperty] public Detalles_Pedidos? Filtro { get; set; }
        [BindProperty] public Detalles_Pedidos? Actual { get; set; }
        [BindProperty] public bool Formulario { get; set; } = false;
        [BindProperty] public bool Borrar { get; set; } = false;

        public void OnGet() { OnPostBtBuscar(); }

        public void OnPostBtBuscar()
        {
            try
            {
                if (!Sesion()) return;

                this.Filtro!.Filtro = this.Filtro!.Filtro ?? "";
                Lista = this.IDetalles_PedidosPresentacion.Buscar(this.Filtro!);
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
                this.Actual = new Detalles_Pedidos();
                this.Pedidos = this.IPedidosPresentacion.Buscar(new Pedidos { Codigo = "" });
                this.Variantes = this.IVariantesPresentacion.Buscar(new Variantes { Sku = "" });
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
                    this.Actual = this.IDetalles_PedidosPresentacion.Insertar(this.Actual!);
                else
                    this.Actual = this.IDetalles_PedidosPresentacion.Actualizar(this.Actual!);
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
                this.Pedidos = this.IPedidosPresentacion.Buscar(new Pedidos { Codigo = "" });
                this.Variantes = this.IVariantesPresentacion.Buscar(new Variantes { Sku = "" });
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
                this.Actual = this.IDetalles_PedidosPresentacion.Borrar(this.Actual!);
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