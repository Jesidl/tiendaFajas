using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class PedidosAplicacion : IPedidosAplicacion
    {
        private IConexion IConexion;

        public PedidosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Pedidos Actualizar(Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Pedidos>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Pedidos Borrar(Pedidos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Pedidos!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Pedidos> Buscar(Pedidos entidad)
        {
            return this.IConexion.Pedidos!
                .Include(x => x._Proveedor)
                .Include(x => x._Empleado)
                .Where(x => x.Codigo!.Contains(entidad.Codigo!))
                .ToList();
        }

        public List<Pedidos> Consultar()
        {
            return this.IConexion.Pedidos!
                .Include(x => x._Proveedor)
                .Include(x => x._Empleado)
                .Take(5).ToList();
        }

        public Pedidos Insertar(Pedidos entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Pedidos!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Pedidos entidad)
        {
            return string.IsNullOrEmpty(entidad.Codigo) ||
                entidad.Proveedor == 0 ||
                entidad.Empleado == 0 ||
                entidad.Fecha == null ||
                string.IsNullOrEmpty(entidad.Estado);
        }
    }
}