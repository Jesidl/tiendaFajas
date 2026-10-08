using libreria_notas.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_notas.interfaces
{
    public class EstudiantesAplicacion : IEstudiantesAplicacion
    {
        private IConexion conexion;

        public EstudiantesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Estudiantes Actualizar(Estudiantes entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Estudiantes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Estudiantes Borrar(Estudiantes entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Estudiantes!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Estudiantes> Consultar()
        {
            return this.conexion.Estudiantes!
                .Where(x => x.Estado)
                .ToList();
        }

        public Estudiantes Insertar(Estudiantes entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Estudiantes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}