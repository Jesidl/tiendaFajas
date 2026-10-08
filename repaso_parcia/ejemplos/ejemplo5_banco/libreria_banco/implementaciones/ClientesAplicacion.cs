using libreria_banco.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_banco.interfaces
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
            var repetido = this.conexion.Clientes!
                .Any(x => x.Documento == entidad.Documento && x.Id != entidad.Id);
            if (repetido)
                throw new Exception("Ya existe otro cliente con ese documento");

            var entry = this.conexion!.Entry<Clientes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Clientes Borrar(Clientes entidad)
        {
            var cliente = this.conexion.Clientes!.FirstOrDefault(x => x.Id == entidad.Id);
            if (cliente == null)
                throw new Exception("El cliente no existe");

            // Regla: no se puede retirar un cliente con cuentas abiertas
            var tieneCuentas = this.conexion.Cuentas!.Any(x => x.Cliente == cliente.Id && x.Activa);
            if (tieneCuentas)
                throw new Exception("El cliente tiene cuentas activas");

            cliente.Activo = false;
            var entry = this.conexion.Entry<Clientes>(cliente);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return cliente;
        }

        public List<Clientes> Consultar()
        {
            // PATRON: INCLUDE DE UNA LISTA (lado "1" trae sus "N")
            // Antes incluiamos el padre desde el hijo (venta._Cliente).
            // Aqui es al reves: el cliente trae su lista de cuentas.
            var lista = this.conexion.Clientes!
                .Include(x => x.Cuentas)
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToList();

            // Ciclo: cliente -> cuentas -> cuenta._Cliente -> cliente ...
            // Se corta del lado de la cuenta.
            foreach (var cliente in lista)
            {
                foreach (var cuenta in cliente.Cuentas!)
                {
                    cuenta._Cliente = null;
                }
            }
            return lista;
        }

        public Clientes Insertar(Clientes entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Documento) || string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new Exception("Documento y nombre son obligatorios");

            var repetido = this.conexion.Clientes!.Any(x => x.Documento == entidad.Documento);
            if (repetido)
                throw new Exception("Ya existe un cliente con ese documento");

            entidad.Activo = true;

            this.conexion.Clientes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}
