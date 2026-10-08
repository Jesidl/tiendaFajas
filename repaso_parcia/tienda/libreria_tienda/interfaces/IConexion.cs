using libreria_tienda.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_tienda.interfaces
{
    public interface IConexion
    {
        string? StringConexion { get; set; }

        DbSet<Productos>? Productos { get; set; }
        DbSet<Ventas>? Ventas { get; set; }

        DbSet<Clientes>? Clientes { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
