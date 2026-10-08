using libreria_banco.entidades;
using libreria_banco.interfaces;
using libreria_banco.nucleo;
using Microsoft.EntityFrameworkCore;

namespace libreria_banco.implementaciones
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

        public DbSet<Clientes>? Clientes { get; set; }
        public DbSet<Cuentas>? Cuentas { get; set; }
        public DbSet<Movimientos>? Movimientos { get; set; }
    }
}
