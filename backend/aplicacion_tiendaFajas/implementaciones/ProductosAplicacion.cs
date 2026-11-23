using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class ProductosAplicacion : IProductosAplicacion
    {
        private IConexion IConexion;

        public ProductosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Productos Actualizar(Productos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Productos>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Productos Borrar(Productos entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Productos!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Productos> Buscar(Productos entidad)
        {
            return this.IConexion.Productos!
                .Include(x => x._Categoria)
                .Include(x => x._Marca)
                .Include(x => x._Material)
                .Where(x => x.Nombre!.Contains(entidad.Nombre!))
                .ToList();
        }

        public List<Productos> Consultar()
        {
            return this.IConexion.Productos!
                .Include(x => x._Categoria)
                .Include(x => x._Marca)
                .Include(x => x._Material)
                .Take(5).ToList();
        }

        public Productos Insertar(Productos entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Productos!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Productos entidad)
        {
            return string.IsNullOrEmpty(entidad.Codigo) ||
                string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                entidad.Categoria == 0 ||
                entidad.Marca == 0 ||
                entidad.Material == 0;
        }
    }
}