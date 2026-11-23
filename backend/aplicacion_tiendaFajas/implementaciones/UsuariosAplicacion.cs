using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace aplicacion_tiendaFajas.interfaces
{
    public class UsuariosAplicacion : IUsuariosAplicacion
    {
        private IConexion IConexion;

        // "sal.hash" en hexadecimal: 32 digitos, punto, 64 digitos
        private static readonly Regex YaDerivada =
            new Regex("^[0-9A-Fa-f]{32}\\.[0-9A-Fa-f]{64}$", RegexOptions.Compiled);

        public UsuariosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Usuarios Actualizar(Usuarios entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            Derivar(entidad);

            var entry = this.IConexion!.Entry<Usuarios>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Usuarios Borrar(Usuarios entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Usuarios!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Usuarios> Buscar(Usuarios entidad)
        {
            return this.IConexion.Usuarios!
                .Include(x => x._Rol)
                .Where(x => x.Correo!.Contains(entidad.Correo!))
                .ToList();
        }

        public List<Usuarios> Consultar()
        {
            return this.IConexion.Usuarios!
                .Include(x => x._Rol)
                .Take(5).ToList();
        }

        public Usuarios Insertar(Usuarios entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            Derivar(entidad);

            this.IConexion.Usuarios!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        // La contrasena solo se deriva aqui, en el servicio. El front nunca
        // calcula el hash: manda la contrasena y recibe de vuelta la derivada.
        // Si el valor que llega ya tiene el formato "sal.hash" es porque el
        // usuario se edito sin cambiar la contrasena, y volver a derivarla
        // dejaria al usuario sin poder entrar.
        private static void Derivar(Usuarios entidad)
        {
            if (string.IsNullOrEmpty(entidad.Clave))
                throw new Exception("Falta informacion");

            if (YaDerivada.IsMatch(entidad.Clave!))
                return;

            entidad.Clave = Clave.Derivar(entidad.Clave!);
        }

        private bool Validar(Usuarios entidad)
        {
            return string.IsNullOrEmpty(entidad.Correo) ||
                string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Clave) ||
                entidad.Rol == 0;
        }
    }
}
