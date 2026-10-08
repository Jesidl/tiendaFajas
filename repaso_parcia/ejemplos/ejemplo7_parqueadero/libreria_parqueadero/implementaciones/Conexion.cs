using libreria_parqueadero.entidades;
using libreria_parqueadero.interfaces;
using libreria_parqueadero.nucleo;
using Microsoft.EntityFrameworkCore;

namespace libreria_parqueadero.implementaciones
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

        public DbSet<Vehiculos>? Vehiculos { get; set; }
        public DbSet<Celdas>? Celdas { get; set; }
        public DbSet<Registros>? Registros { get; set; }
    }
}
