using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class MedidasAplicacion : IMedidasAplicacion
    {
        private IConexion IConexion;

        public MedidasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Medidas Actualizar(Medidas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Medidas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Medidas Borrar(Medidas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Medidas!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Medidas> Buscar(Medidas entidad)
        {
            return this.IConexion.Medidas!
                .Include(x => x._Cliente)
                .Where(x => x._Cliente!.Nombre!.Contains(entidad.Filtro!))
                .ToList();
        }

        public List<Medidas> Consultar()
        {
            return this.IConexion.Medidas!
                .Include(x => x._Cliente)
                .Take(5).ToList();
        }

        public Medidas Insertar(Medidas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Medidas!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Medidas entidad)
        {
            return entidad.Cliente == 0 ||
                entidad.Fecha == null;
        }
    }
}