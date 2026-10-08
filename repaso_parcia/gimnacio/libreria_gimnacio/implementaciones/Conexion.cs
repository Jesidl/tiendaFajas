using libreria_gimnacio.entidades;
using libreria_gimnacio.interfaces;
using libreria_gimnacio.nucleo;
using Microsoft.EntityFrameworkCore;




namespace libreria_gimnacio.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.StringConexion ?? DatosGenerales.ObtenerStringConexion(), p => { });

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Planes>? Planes { get; set; }
        public DbSet<Inscripciones>? Inscripciones { get; set; }

        public DbSet<Socios>? Socios { get; set; }
    }
}
