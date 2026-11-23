using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class VentasAplicacion : IVentasAplicacion
    {
        private IConexion IConexion;

        public VentasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Ventas Actualizar(Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Ventas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Ventas Borrar(Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Ventas!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Ventas> Buscar(Ventas entidad)
        {
            return this.IConexion.Ventas!
                .Include(x => x._Cliente)
                .Include(x => x._Empleado)
                .Where(x => x.Codigo!.Contains(entidad.Codigo!))
                .ToList();
        }

        public List<Ventas> Consultar()
        {
            return this.IConexion.Ventas!
                .Include(x => x._Cliente)
                .Include(x => x._Empleado)
                .Take(5).ToList();
        }

        public Ventas Insertar(Ventas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Ventas!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Ventas entidad)
        {
            return string.IsNullOrEmpty(entidad.Codigo) ||
                entidad.Cliente == 0 ||
                entidad.Empleado == 0 ||
                entidad.Fecha == null;
        }
    }
}