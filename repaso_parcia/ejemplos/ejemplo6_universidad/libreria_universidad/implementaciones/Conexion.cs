using libreria_universidad.entidades;
using libreria_universidad.interfaces;
using libreria_universidad.nucleo;
using Microsoft.EntityFrameworkCore;

namespace libreria_universidad.implementaciones
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

        public DbSet<Estudiantes>? Estudiantes { get; set; }
        public DbSet<Cursos>? Cursos { get; set; }
        public DbSet<Matriculas>? Matriculas { get; set; }
    }
}
