using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace front_tiendaFajas.Pages
{
    public class IndexModel : PageModel
    {
        private IUsuariosPresentacion IUsuariosPresentacion;

        public IndexModel(IUsuariosPresentacion IUsuariosPresentacion)
        {
            this.IUsuariosPresentacion = IUsuariosPresentacion;
        }

        public bool EstaLogueado { get; set; } = false;
        public string? Usuario { get; set; }
        public string? Rol { get; set; }

        [BindProperty] public string? Correo { get; set; }
        [BindProperty] public string? ClaveEscrita { get; set; }

        public void OnGet()
        {
            Revisar();
        }

        public void OnPostBtLimpiar()
        {
            try
            {
                Correo = string.Empty;
                ClaveEscrita = string.Empty;
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtEntrar()
        {
            try
            {
                if (string.IsNullOrEmpty(Correo) ||
                    string.IsNullOrEmpty(ClaveEscrita))
                {
                    ViewData["Mensaje"] = "Escriba el correo y la contrasena";
                    return;
                }

                var encontrados = this.IUsuariosPresentacion.Buscar(
                    new Usuarios { Correo = Correo });

                var usuario = encontrados
                    .FirstOrDefault(x => x.Correo == Correo);

                // mismo mensaje para usuario inexistente y clave equivocada:
                // no se le dice a un atacante cuales correos existen
                if (usuario == null ||
                    !Clave.Verificar(ClaveEscrita, usuario.Clave))
                {
                    ViewData["Mensaje"] = "Correo o contrasena incorrectos";
                    OnPostBtLimpiar();
                    return;
                }

                if (!usuario.Estado)
                {
                    ViewData["Mensaje"] = "El usuario esta inactivo";
                    OnPostBtLimpiar();
                    return;
                }

                HttpContext.Session.SetString("Usuario", usuario.Correo!);
                HttpContext.Session.SetString("Nombre", usuario.Nombre ?? "");
                HttpContext.Session.SetString("Rol", usuario._Rol?.Nombre ?? "");

                OnPostBtLimpiar();
                Revisar();
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtCerrar()
        {
            try
            {
                HttpContext.Session.Clear();
                EstaLogueado = false;
                Usuario = null;
                Rol = null;
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        private void Revisar()
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (String.IsNullOrEmpty(usuario))
                return;

            EstaLogueado = true;
            Usuario = HttpContext.Session.GetString("Nombre");
            if (String.IsNullOrEmpty(Usuario))
                Usuario = usuario;
            Rol = HttpContext.Session.GetString("Rol");
        }
    }
}
