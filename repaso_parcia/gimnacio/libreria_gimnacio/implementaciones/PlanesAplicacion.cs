using libreria_gimnacio
    .entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_gimnacio.interfaces
{
    public class PlanesAplicacion : IPlanesAplicacion
    {
        private IConexion conexion;

        public PlanesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Planes Actualizar(Planes entidad)
        {
            // Hacer calculo o operaciones

            Validar(entidad);

            var entry = this.conexion!.Entry<Planes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Planes Borrar(Planes entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Planes!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Planes> Consultar()
        {
            return this.conexion.Planes!
                .ToList();
        }

        public Planes Insertar(Planes entidad)
        {
            // Hacer calculo o operaciones

            Validar(entidad);
            this.conexion.Planes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Planes entidad)
        {
            if (entidad.DuracionDias <= 0)
                throw new Exception("La duración en días debe ser mayor a cero");
            if (entidad.Precio <= 0)
                throw new Exception("El precio debe ser mayor a cero");
            return true;
        }
    }
}