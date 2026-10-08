using libreria_hotel.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_hotel.interfaces
{
    public class HuespedesAplicacion : IHuespedesAplicacion
    {
        private IConexion conexion;

        public HuespedesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Huespedes Actualizar(Huespedes entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Huespedes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Huespedes Borrar(Huespedes entidad)
        {
            // Hacer calculo o operaciones


            if (entidad.Reservas != null && entidad.Reservas.Count > 0)
            {
                throw new InvalidOperationException("No se puede eliminar el cliente porque tiene reservas asociadas.");
            }

            this.conexion.Huespedes!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Huespedes> Consultar()
        {  
            return this.conexion.Huespedes!
                .Where(x => x.Activo)
                .ToList();
        }

        public Huespedes Insertar(Huespedes  entidad)
        {
            // Hacer calculo o operaciones
             entidad.Activo = true;
            entidad.Frecuente = false;

            this.conexion.Huespedes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}