using libreria_banco.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_banco.interfaces
{
    // Contrato de la conexion. Las clases ...Aplicacion reciben ESTA interfaz
    // (no la clase Conexion) -> inyeccion de dependencias / desacople.
    public interface IConexion
    {
        string? StringConexion { get; set; }

        // Un DbSet por cada tabla
        DbSet<Clientes>? Clientes { get; set; }
        DbSet<Cuentas>? Cuentas { get; set; }
        DbSet<Movimientos>? Movimientos { get; set; }

        // Entry: para marcar un objeto como Modificado (Actualizar)
        EntityEntry<T> Entry<T>(T entity) where T : class;
        // SaveChanges: envia a la base todo lo pendiente en UNA transaccion
        int SaveChanges();
    }
}
