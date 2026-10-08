using libreria_gimnacio.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_gimnacio.interfaces
{
    public class SociosAplicacion : ISociosAplicacion
    {
        private IConexion conexion;

        public SociosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Socios Actualizar(Socios entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Socios>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Socios Borrar(Socios entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Socios!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Socios> Consultar()
        {
            return this.conexion.Socios!
                .Where(x => x.Activo)
                .ToList();
        }

        public Socios Insertar(Socios entidad)
        {
            // Hacer calculo o operaciones

            entidad.Activo = true;

            this.conexion.Socios!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}