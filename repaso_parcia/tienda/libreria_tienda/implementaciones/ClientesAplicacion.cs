using libreria_tienda.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_tienda.interfaces
{
    public class ClientesAplicacion : IClientesAplicacion
    {
        private IConexion conexion;

        public ClientesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Clientes Actualizar(Clientes entidad)
        {
            // Hacer calculo o operaciones

            var entry = this.conexion!.Entry<Clientes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Clientes Borrar(Clientes entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Clientes!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Clientes> Consultar()
        {
            return this.conexion.Clientes!
                .Where(x => x.Activo)
                .ToList();
        }

        public Clientes Insertar(Clientes entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Clientes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}