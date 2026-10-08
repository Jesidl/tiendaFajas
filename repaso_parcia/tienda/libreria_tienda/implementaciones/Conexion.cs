using libreria_tienda.entidades;
using libreria_tienda.interfaces;
using libreria_tienda.nucleo;
using Microsoft.EntityFrameworkCore;




namespace libreria_tienda.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.StringConexion ?? DatosGenerales.ObtenerStringConexion(), p => { });

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Productos>? Productos { get; set; }
        public DbSet<Ventas>? Ventas { get; set; }

        public DbSet<Clientes>? Clientes { get; set; }
    }
}
