using libreria_hotel.entidades;
using libreria_hotel.interfaces;
using libreria_hotel.nucleo;
using Microsoft.EntityFrameworkCore;




namespace libreria_hotel.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.StringConexion ?? DatosGenerales.ObtenerStringConexion(), p => { });

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Habitaciones>? Habitaciones { get; set; }
        public DbSet<Reservas>? Reservas { get; set; }

        public DbSet<Huespedes>? Huespedes { get; set; }
    }
}
