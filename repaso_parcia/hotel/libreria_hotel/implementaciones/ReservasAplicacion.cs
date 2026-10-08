using libreria_hotel.entidades;
using libreria_hotel.implementaciones;
using Microsoft.EntityFrameworkCore;

namespace libreria_hotel.interfaces
{
    public class ReservasAplicacion : IReservasAplicacion
    {
        private IConexion conexion;

        public ReservasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Reservas Actualizar(Reservas entidad)
        {
           

            var entry = this.conexion!.Entry<Reservas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Reservas Borrar(Reservas entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Reservas!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Reservas> Consultar()
        {
            var lista = this.conexion.Reservas!
                .Include(x => x._Habitacion)
                .Include(x => x._Huesped)
                .ToList();

            foreach (var item in lista)
            {
                item._Huesped!.Reservas = null;
                item._Habitacion!.Reservas = null;
            }
            return lista;
        }

        public Reservas Insertar(Reservas entidad)
        {


            // 2. Si vienen cliente o producto NUEVOS, se guardan primero
            if (entidad._Huesped != null)
                this.conexion.Huespedes!.Add(entidad._Huesped);

            if (entidad._Habitacion != null)
                this.conexion.Habitaciones!.Add(entidad._Habitacion);

            // 3. Guardar la venta
            this.conexion.Reservas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}