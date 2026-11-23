using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class VariantesAplicacion : IVariantesAplicacion
    {
        private IConexion IConexion;

        public VariantesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Variantes Actualizar(Variantes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Variantes>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Variantes Borrar(Variantes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Variantes!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Variantes> Buscar(Variantes entidad)
        {
            return this.IConexion.Variantes!
                .Include(x => x._Producto)
                .Include(x => x._Talla)
                .Include(x => x._Color)
                .Where(x => x.Sku!.Contains(entidad.Sku!))
                .ToList();
        }

        public List<Variantes> Consultar()
        {
            return this.IConexion.Variantes!
                .Include(x => x._Producto)
                .Include(x => x._Talla)
                .Include(x => x._Color)
                .Take(5).ToList();
        }

        public Variantes Insertar(Variantes entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Variantes!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Variantes entidad)
        {
            return string.IsNullOrEmpty(entidad.Sku) ||
                entidad.Producto == 0 ||
                entidad.Talla == 0 ||
                entidad.Color == 0;
        }
    }
}