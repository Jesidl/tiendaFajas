using libreria_notas.entidades;
using libreria_notas.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_notas.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.StringConexion!, p => { });
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Estudiantes>? Estudiantes { get; set; }
        public DbSet<Materias>? Materias { get; set; }
        public DbSet<Notas>? Notas { get; set; }
    }
}
