using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class Detalles_VentasAplicacion : IDetalles_VentasAplicacion
    {
        private IConexion IConexion;

        public Detalles_VentasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Detalles_Ventas Actualizar(Detalles_Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Detalles_Ventas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Detalles_Ventas Borrar(Detalles_Ventas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Detalles_Ventas!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Detalles_Ventas> Buscar(Detalles_Ventas entidad)
        {
            return this.IConexion.Detalles_Ventas!
                .Include(x => x._Venta)
                .Include(x => x._Variante)
                .Where(x => x._Venta!.Codigo!.Contains(entidad.Filtro!))
                .ToList();
        }

        public List<Detalles_Ventas> Consultar()
        {
            return this.IConexion.Detalles_Ventas!
                .Include(x => x._Venta)
                .Include(x => x._Variante)
                .Take(5).ToList();
        }

        public Detalles_Ventas Insertar(Detalles_Ventas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Detalles_Ventas!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Detalles_Ventas entidad)
        {
            return entidad.Venta == 0 ||
                entidad.Variante == 0;
        }
    }
}