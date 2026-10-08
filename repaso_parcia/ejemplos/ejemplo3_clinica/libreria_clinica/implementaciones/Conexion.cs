using libreria_clinica.entidades;
using libreria_clinica.interfaces;
using libreria_clinica.nucleo;
using Microsoft.EntityFrameworkCore;

namespace libreria_clinica.implementaciones
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

        public DbSet<Pacientes>? Pacientes { get; set; }
        public DbSet<Medicos>? Medicos { get; set; }
        public DbSet<Citas>? Citas { get; set; }
    }
}
