using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Datosgenerales.ObtenerStringConexion()!, p => { });
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Cargos>? Cargos { get; set; }
        public DbSet<Sucursales>? Sucursales { get; set; }
        public DbSet<Clientes>? Clientes { get; set; }
        public DbSet<Proveedores>? Proveedores { get; set; }
        public DbSet<Categorias>? Categorias { get; set; }
        public DbSet<Marcas>? Marcas { get; set; }
        public DbSet<Materiales>? Materiales { get; set; }
        public DbSet<Tallas>? Tallas { get; set; }
        public DbSet<Colores>? Colores { get; set; }
        public DbSet<Metodos_Pago>? Metodos_Pago { get; set; }
        public DbSet<Empleados>? Empleados { get; set; }
        public DbSet<Medidas>? Medidas { get; set; }
        public DbSet<Productos>? Productos { get; set; }
        public DbSet<Variantes>? Variantes { get; set; }
        public DbSet<Inventarios>? Inventarios { get; set; }
        public DbSet<Ventas>? Ventas { get; set; }
        public DbSet<Detalles_Ventas>? Detalles_Ventas { get; set; }
        public DbSet<Pagos>? Pagos { get; set; }
        public DbSet<Pedidos>? Pedidos { get; set; }
        public DbSet<Detalles_Pedidos>? Detalles_Pedidos { get; set; }
        public DbSet<Roles>? Roles { get; set; }
        public DbSet<Usuarios>? Usuarios { get; set; }
        public DbSet<Permisos>? Permisos { get; set; }
        public DbSet<Auditorias>? Auditorias { get; set; }
        public DbSet<Imagenes>? Imagenes { get; set; }

        // ------------------------------------------------------------------
        // Auditoria transversal.
        //
        // Se escribe aqui y no en los 20 XAplicacion para no repetir la misma
        // logica 60 veces (Insertar, Actualizar y Borrar de cada entidad).
        // Al interceptar SaveChanges se audita toda escritura que pase por la
        // conexion, incluidas las que se agreguen despues, sin tocar ninguna
        // clase de negocio.
        //
        // Las dos escrituras van dentro de una transaccion para que no pueda
        // quedar una fila guardada sin su rastro de auditoria.
        // ------------------------------------------------------------------
        public override int SaveChanges()
        {
            var pendientes = Recolectar();

            if (pendientes.Count == 0)
                return base.SaveChanges();

            var propia = this.Database.CurrentTransaction == null;
            var transaccion = propia
                ? this.Database.BeginTransaction()
                : this.Database.CurrentTransaction;

            try
            {
                var filas = base.SaveChanges();

                foreach (var par in pendientes)
                    par.Auditoria.Llave = Llave(par.Entidad);

                var escritas = pendientes.Select(x => x.Auditoria).ToList();
                this.Auditorias!.AddRange(escritas);
                base.SaveChanges();

                // Se sueltan del ChangeTracker. Para esta conexion la bitacora
                // es de solo escritura: nadie vuelve a necesitar esas filas
                // rastreadas, y dejarlas tiene dos consecuencias.
                //
                // La primera es que el contexto acumula cada fila de auditoria
                // que haya escrito y nunca la libera.
                //
                // La segunda es peor: si despues alguien trabaja con esas
                // mismas filas en la misma conexion -por ejemplo la API de
                // Auditorias, que tambien expone Actualizar y Borrar-, Entity
                // Framework se niega con "another instance with the same key
                // value is already being tracked", porque la fila que llega de
                // una consulta es una instancia distinta con el mismo Id.
                foreach (var fila in escritas)
                    this.Entry(fila).State = EntityState.Detached;

                if (propia)
                    transaccion!.Commit();

                return filas;
            }
            catch
            {
                if (propia)
                    transaccion!.Rollback();
                throw;
            }
        }

        private List<(Auditorias Auditoria, object Entidad)> Recolectar()
        {
            var salida = new List<(Auditorias, object)>();

            foreach (var entrada in this.ChangeTracker.Entries().ToList())
            {
                // la auditoria no se audita a si misma: seria recursivo
                if (entrada.Entity is Auditorias)
                    continue;

                var operacion = entrada.State switch
                {
                    EntityState.Added => "Insertar",
                    EntityState.Modified => "Actualizar",
                    EntityState.Deleted => "Borrar",
                    _ => null
                };

                if (operacion == null)
                    continue;

                salida.Add((new Auditorias
                {
                    Entidad = entrada.Entity.GetType().Name,
                    Llave = 0,
                    Operacion = operacion,
                    Usuario = ContextoUsuario.Nombre(),
                    Fecha = DateTime.Now,
                    Datos = Datos(entrada)
                }, entrada.Entity));
            }

            return salida;
        }

        // El texto se arma como "campo=valor; campo=valor" y no como JSON a
        // proposito: el metodo Replace() de Comunicaciones destroza llaves,
        // corchetes y comillas, asi que un JSON anidado llegaria ilegible al
        // front. Este formato no usa ninguno de esos caracteres.
        private static string Datos(EntityEntry entrada)
        {
            var texto = new StringBuilder();

            foreach (var propiedad in entrada.Properties)
            {
                var nombre = propiedad.Metadata.Name;

                // jamas registrar el hash de la contrasena en la bitacora
                if (nombre == "Clave")
                    continue;

                // los binarios no se registran: un VARBINARY(MAX) no cabe en
                // la columna de datos y no dice nada legible. El tamaño del
                // archivo ya queda en su propia columna.
                if (propiedad.Metadata.ClrType == typeof(byte[]))
                    continue;

                // el Id ya viaja en Llave, y aqui todavia no es el definitivo:
                // en una insercion EF aun tiene el temporal negativo, porque
                // este texto se arma antes de que SaveChanges llegue a la base
                if (nombre == "Id")
                    continue;

                var valor = entrada.State == EntityState.Deleted
                    ? propiedad.OriginalValue
                    : propiedad.CurrentValue;

                if (texto.Length > 0)
                    texto.Append("; ");

                texto.Append(nombre);
                texto.Append('=');
                texto.Append(valor?.ToString()?.Replace(';', ',') ?? "");

                if (texto.Length > 3000)
                    break;
            }

            return texto.ToString();
        }

        private static int Llave(object entidad)
        {
            var propiedad = entidad.GetType().GetProperty("Id");
            if (propiedad == null)
                return 0;

            var valor = propiedad.GetValue(entidad);
            return valor == null ? 0 : Convert.ToInt32(valor);
        }
    }
}
