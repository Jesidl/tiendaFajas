using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class InventariosAplicacion : IInventariosAplicacion
    {
        private IConexion IConexion;

        public InventariosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Inventarios Actualizar(Inventarios entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Inventarios>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Inventarios Borrar(Inventarios entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Inventarios!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Inventarios> Buscar(Inventarios entidad)
        {
            return this.IConexion.Inventarios!
                .Include(x => x._Variante)
                .Include(x => x._Sucursal)
                .Where(x => x._Variante!.Sku!.Contains(entidad.Filtro!))
                .ToList();
        }

        public List<Inventarios> Consultar()
        {
            return this.IConexion.Inventarios!
                .Include(x => x._Variante)
                .Include(x => x._Sucursal)
                .Take(5).ToList();
        }

        public Inventarios Insertar(Inventarios entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Inventarios!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Inventarios entidad)
        {
            return entidad.Variante == 0 ||
                entidad.Sucursal == 0 ||
                entidad.Fecha_Actualizacion == null;
        }
    }
}