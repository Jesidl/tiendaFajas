using libreria_hotel.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace libreria_hotel.interfaces
{
    public interface IConexion
    {
        string? StringConexion { get; set; }

        DbSet<Habitaciones>? Habitaciones { get; set; }
        DbSet<Reservas>? Reservas { get; set; }

        DbSet<Huespedes>? Huespedes { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
