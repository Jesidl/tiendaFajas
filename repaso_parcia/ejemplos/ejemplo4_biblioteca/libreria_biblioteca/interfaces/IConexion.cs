using libreria_biblioteca.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_biblioteca.interfaces
{
    // Contrato de la conexion. Las clases ...Aplicacion reciben ESTA interfaz
    // (no la clase Conexion) -> inyeccion de dependencias / desacople.
    public interface IConexion
    {
        string? StringConexion { get; set; }

        // Un DbSet por cada tabla
        DbSet<Libros>? Libros { get; set; }
        DbSet<Lectores>? Lectores { get; set; }
        DbSet<Prestamos>? Prestamos { get; set; }

        // Entry: para marcar un objeto como Modificado (Actualizar)
        EntityEntry<T> Entry<T>(T entity) where T : class;
        // SaveChanges: envia a la base todo lo pendiente en UNA transaccion
        int SaveChanges();
    }
}
