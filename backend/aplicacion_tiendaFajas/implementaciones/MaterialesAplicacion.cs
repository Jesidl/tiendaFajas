using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class MaterialesAplicacion : IMaterialesAplicacion
    {
        private IConexion IConexion;

        public MaterialesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Materiales Actualizar(Materiales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Materiales>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Materiales Borrar(Materiales entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Materiales!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Materiales> Buscar(Materiales entidad)
        {
            return this.IConexion.Materiales!
                .Where(x => x.Nombre!.Contains(entidad.Nombre!))
                .ToList();
        }

        public List<Materiales> Consultar()
        {
            return this.IConexion.Materiales!.Take(5).ToList();
        }

        public Materiales Insertar(Materiales entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Materiales!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Materiales entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}