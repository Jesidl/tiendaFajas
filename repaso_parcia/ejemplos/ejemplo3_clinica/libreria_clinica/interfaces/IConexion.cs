using libreria_clinica.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_clinica.interfaces
{
    // Contrato de la conexion. Las clases ...Aplicacion reciben ESTA interfaz
    // (no la clase Conexion) -> inyeccion de dependencias / desacople.
    public interface IConexion
    {
        string? StringConexion { get; set; }

        // Un DbSet por cada tabla
        DbSet<Pacientes>? Pacientes { get; set; }
        DbSet<Medicos>? Medicos { get; set; }
        DbSet<Citas>? Citas { get; set; }

        // Entry: para marcar un objeto como Modificado (Actualizar)
        EntityEntry<T> Entry<T>(T entity) where T : class;
        // SaveChanges: envia a la base todo lo pendiente en UNA transaccion
        int SaveChanges();
    }
}
