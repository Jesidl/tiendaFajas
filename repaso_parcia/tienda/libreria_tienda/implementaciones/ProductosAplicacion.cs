using libreria_tienda.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_tienda.interfaces
{
    public class ProductosAplicacion : IProductosAplicacion
    {
        private IConexion conexion;

        public ProductosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Productos Actualizar(Productos entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Productos>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Productos Borrar(Productos entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Productos!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Productos> Consultar()
        {
            return this.conexion.Productos!
                .ToList();
        }

        public Productos Insertar(Productos entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Productos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}