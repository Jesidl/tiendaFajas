using libreria_notas.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_notas.interfaces
{
    public interface IConexion
    {
        string? StringConexion { get; set; }

        DbSet<Estudiantes>? Estudiantes { get; set; }
        DbSet<Notas>? Notas { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
