using libreria_notas.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_notas.interfaces
{
    public class NotasAplicacion : INotasAplicacion
    {
        private IConexion conexion;

        public NotasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Notas Actualizar(Notas entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Notas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Notas Borrar(Notas entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Notas!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Notas> Consultar()
        {
            var lista = this.conexion.Notas!
                .Include(x => x._Estudiante)
                .Include(x => x._Materia)
                .ToList();

            foreach (var item in lista)
            {
                item._Estudiante!.Notas = null;
                item._Materia!.Notas = null;
            }
            return lista;
        }

        public Notas Insertar(Notas entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Notas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}