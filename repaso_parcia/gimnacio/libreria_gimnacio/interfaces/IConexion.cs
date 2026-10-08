using libreria_gimnacio.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_gimnacio.interfaces
{
    public interface IConexion
    {
        string? StringConexion { get; set; }

        DbSet<Planes>? Planes { get; set; }
        DbSet<Inscripciones>? Inscripciones { get; set; }

        DbSet<Socios>? Socios { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
