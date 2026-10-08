using libreria_hotel.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_hotel.interfaces
{
    public class HabitacionesAplicacion : IHabitacionesAplicacion
    {
        private IConexion conexion;

        public HabitacionesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Habitaciones Actualizar(Habitaciones entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Habitaciones>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Habitaciones Borrar(Habitaciones entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Habitaciones!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Habitaciones> Consultar()
        {
            return this.conexion.Habitaciones!
                .ToList();
        }

        public Habitaciones Insertar(Habitaciones entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Habitaciones!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}