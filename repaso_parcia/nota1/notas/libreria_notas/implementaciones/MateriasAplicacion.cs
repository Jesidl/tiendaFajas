using libreria_notas.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_notas.interfaces
{
    public class MateriasAplicacion : IMateriasAplicacion
    {
        private IConexion conexion;

        public MateriasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Materias Actualizar(Materias entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Materias>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Materias Borrar(Materias entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Materias!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Materias> Consultar()
        {
            return this.conexion.Materias!
                .ToList();
        }

        public Materias Insertar(Materias entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Materias!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}