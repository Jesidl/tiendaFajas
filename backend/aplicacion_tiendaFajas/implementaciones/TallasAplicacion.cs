using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class TallasAplicacion : ITallasAplicacion
    {
        private IConexion IConexion;

        public TallasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Tallas Actualizar(Tallas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Tallas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Tallas Borrar(Tallas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Tallas!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Tallas> Buscar(Tallas entidad)
        {
            return this.IConexion.Tallas!
                .Where(x => x.Nombre!.Contains(entidad.Nombre!))
                .ToList();
        }

        public List<Tallas> Consultar()
        {
            return this.IConexion.Tallas!
                .Take(5).ToList();
        }

        public Tallas Insertar(Tallas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Tallas!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Tallas entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}