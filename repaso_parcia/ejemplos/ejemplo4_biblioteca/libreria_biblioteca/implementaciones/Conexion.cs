using libreria_biblioteca.entidades;
using libreria_biblioteca.interfaces;
using libreria_biblioteca.nucleo;
using Microsoft.EntityFrameworkCore;

namespace libreria_biblioteca.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ?? -> si nadie asigno StringConexion (caso Update-Database) usa DatosGenerales
            optionsBuilder.UseSqlServer(this.StringConexion ?? DatosGenerales.ObtenerStringConexion(), p => { });
            // NoTracking: EF NO vigila los objetos que consulta.
            // Consecuencia: si consultas un registro y le cambias algo, debes marcarlo
            // con Entry(obj).State = EntityState.Modified para que se guarde.
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Libros>? Libros { get; set; }
        public DbSet<Lectores>? Lectores { get; set; }
        public DbSet<Prestamos>? Prestamos { get; set; }
    }
}
