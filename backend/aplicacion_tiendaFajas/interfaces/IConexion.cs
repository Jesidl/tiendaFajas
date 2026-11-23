using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IConexion
    {
        DbSet<Cargos>? Cargos { get; set; }
        DbSet<Sucursales>? Sucursales { get; set; }
        DbSet<Clientes>? Clientes { get; set; }
        DbSet<Proveedores>? Proveedores { get; set; }
        DbSet<Categorias>? Categorias { get; set; }
        DbSet<Marcas>? Marcas { get; set; }
        DbSet<Materiales>? Materiales { get; set; }
        DbSet<Tallas>? Tallas { get; set; }
        DbSet<Colores>? Colores { get; set; }
        DbSet<Metodos_Pago>? Metodos_Pago { get; set; }
        DbSet<Empleados>? Empleados { get; set; }
        DbSet<Medidas>? Medidas { get; set; }
        DbSet<Productos>? Productos { get; set; }
        DbSet<Variantes>? Variantes { get; set; }
        DbSet<Inventarios>? Inventarios { get; set; }
        DbSet<Ventas>? Ventas { get; set; }
        DbSet<Detalles_Ventas>? Detalles_Ventas { get; set; }
        DbSet<Pagos>? Pagos { get; set; }
        DbSet<Pedidos>? Pedidos { get; set; }
        DbSet<Detalles_Pedidos>? Detalles_Pedidos { get; set; }
        DbSet<Roles>? Roles { get; set; }
        DbSet<Usuarios>? Usuarios { get; set; }
        DbSet<Permisos>? Permisos { get; set; }
        DbSet<Auditorias>? Auditorias { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
