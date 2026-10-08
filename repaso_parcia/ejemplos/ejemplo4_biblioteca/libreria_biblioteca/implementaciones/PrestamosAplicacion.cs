using libreria_biblioteca.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_biblioteca.interfaces
{
    public class PrestamosAplicacion : IPrestamosAplicacion
    {
        // PATRON: CONSTANTES DE NEGOCIO
        // Los numeros "magicos" del enunciado se ponen arriba con nombre:
        // si el profe cambia la regla, se cambia en un solo lugar.
        private const int MAX_PRESTAMOS = 3;
        private const int DIAS_PRESTAMO = 15;
        private const decimal MULTA_POR_DIA = 1000m;

        private IConexion conexion;

        public PrestamosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR = PRESTAR UN LIBRO
        // PATRON PRINCIPAL: MODIFICAR OTRA TABLA (stock del libro)
        // =====================================================================
        public Prestamos Insertar(Prestamos entidad)
        {
            // ---- 1. VALORES POR DEFECTO / CALCULADOS ----
            // default(DateTime) = 01/01/0001 -> significa "no la mandaron".
            // Si no viene, el prestamo es HOY. (Si viene, se respeta: util para pruebas.)
            if (entidad.FechaPrestamo == default(DateTime))
                entidad.FechaPrestamo = DateTime.Now;

            entidad.FechaLimite = entidad.FechaPrestamo.AddDays(DIAS_PRESTAMO);
            entidad.FechaDevolucion = null;
            entidad.Devuelto = false;
            entidad.DiasRetraso = 0;
            entidad.Multa = 0;

            // ---- 2. LECTOR ----
            if (entidad._Lector != null)
            {
                // lector NUEVO (caso Parcial)
                entidad._Lector.Activo = true;
                this.conexion.Lectores!.Add(entidad._Lector);
            }
            else
            {
                var lector = this.conexion.Lectores!.FirstOrDefault(x => x.Id == entidad.Lector);
                if (lector == null || !lector.Activo)
                    throw new Exception("El lector no existe o esta inactivo");

                // PATRON: LIMITE DE CANTIDAD -> Count() con condicion
                int pendientes = this.conexion.Prestamos!
                    .Count(x => x.Lector == entidad.Lector && !x.Devuelto);
                if (pendientes >= MAX_PRESTAMOS)
                    throw new Exception("El lector ya tiene " + MAX_PRESTAMOS + " libros prestados");

                // PATRON: BLOQUEO POR MOROSIDAD
                var ahora = DateTime.Now;
                var tieneVencidos = this.conexion.Prestamos!
                    .Any(x => x.Lector == entidad.Lector && !x.Devuelto && x.FechaLimite < ahora);
                if (tieneVencidos)
                    throw new Exception("El lector tiene prestamos vencidos");
            }

            // ---- 3. LIBRO (y su stock) ----
            if (entidad._Libro != null)
            {
                // libro NUEVO: nace con todos sus ejemplares menos el que se presta ahora
                if (entidad._Libro.Ejemplares <= 0)
                    throw new Exception("El libro debe tener ejemplares");
                entidad._Libro.Disponibles = entidad._Libro.Ejemplares - 1;
                this.conexion.Libros!.Add(entidad._Libro);
            }
            else
            {
                var libro = this.conexion.Libros!.FirstOrDefault(x => x.Id == entidad.Libro);
                if (libro == null)
                    throw new Exception("El libro no existe");
                if (libro.Disponibles <= 0)
                    throw new Exception("No hay ejemplares disponibles de " + libro.Titulo);

                var yaLoTiene = this.conexion.Prestamos!
                    .Any(x => x.Libro == entidad.Libro && x.Lector == entidad.Lector && !x.Devuelto);
                if (yaLoTiene)
                    throw new Exception("El lector ya tiene prestado este libro");

                // >>> MODIFICAR OTRA TABLA <<<
                libro.Disponibles = libro.Disponibles - 1;

                // La conexion es NoTracking: EF no "ve" el cambio solo, hay que marcarlo
                var entryLibro = this.conexion.Entry<Libros>(libro);
                entryLibro.State = EntityState.Modified;
            }

            // ---- 4. GUARDAR: el prestamo Y el cambio de stock en UN SaveChanges ----
            // Si algo falla, no queda el stock descontado sin prestamo (transaccion).
            this.conexion.Prestamos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // DEVOLVER: accion de negocio. Calcula multa y devuelve el stock.
        // =====================================================================
        public Prestamos Devolver(int id)
        {
            var prestamo = this.conexion.Prestamos!.FirstOrDefault(x => x.Id == id);
            if (prestamo == null)
                throw new Exception("El prestamo no existe");
            if (prestamo.Devuelto)
                throw new Exception("Este prestamo ya fue devuelto");

            prestamo.FechaDevolucion = DateTime.Now;
            prestamo.Devuelto = true;

            // PATRON: DIFERENCIA ENTRE FECHAS
            // Restar 2 DateTime da un TimeSpan; .Days = dias completos.
            // .Date quita la hora para contar dias de calendario.
            int dias = (prestamo.FechaDevolucion.Value.Date - prestamo.FechaLimite.Date).Days;

            // Math.Max(0, x) -> si devolvio antes de tiempo, dias negativos = 0
            prestamo.DiasRetraso = Math.Max(0, dias);
            prestamo.Multa = prestamo.DiasRetraso * MULTA_POR_DIA;

            // El ejemplar vuelve al estante (otra tabla)
            var libro = this.conexion.Libros!.FirstOrDefault(x => x.Id == prestamo.Libro);
            if (libro != null)
            {
                libro.Disponibles = libro.Disponibles + 1;
                var entryLibro = this.conexion.Entry<Libros>(libro);
                entryLibro.State = EntityState.Modified;
            }

            var entry = this.conexion.Entry<Prestamos>(prestamo);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return prestamo;
        }

        // =====================================================================
        // ACTUALIZAR = RENOVAR
        // PATRON: SOLO CIERTOS CAMPOS SON EDITABLES
        // Se parte del ORIGINAL y solo se copia lo que se permite cambiar.
        // =====================================================================
        public Prestamos Actualizar(Prestamos entidad)
        {
            var original = this.conexion.Prestamos!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("El prestamo no existe");
            if (original.Devuelto)
                throw new Exception("No se puede modificar un prestamo devuelto");

            // Reglas de la renovacion
            if (entidad.FechaLimite <= original.FechaLimite)
                throw new Exception("La nueva fecha limite debe ser posterior a la actual");
            if (entidad.FechaLimite > original.FechaPrestamo.AddDays(DIAS_PRESTAMO * 2))
                throw new Exception("Maximo se puede renovar hasta " + (DIAS_PRESTAMO * 2) + " dias desde el prestamo");

            // Solo cambia la fecha limite; todo lo demas queda como estaba
            original.FechaLimite = entidad.FechaLimite;

            var entry = this.conexion.Entry<Prestamos>(original);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return original;
        }

        public Prestamos Borrar(Prestamos entidad)
        {
            var prestamo = this.conexion.Prestamos!.FirstOrDefault(x => x.Id == entidad.Id);
            if (prestamo == null)
                throw new Exception("El prestamo no existe");

            // Si se borra un prestamo activo, el ejemplar debe volver al stock
            if (!prestamo.Devuelto)
            {
                var libro = this.conexion.Libros!.FirstOrDefault(x => x.Id == prestamo.Libro);
                if (libro != null)
                {
                    libro.Disponibles = libro.Disponibles + 1;
                    var entryLibro = this.conexion.Entry<Libros>(libro);
                    entryLibro.State = EntityState.Modified;
                }
            }

            this.conexion.Prestamos!.Remove(prestamo);
            this.conexion.SaveChanges();
            return prestamo;
        }

        public List<Prestamos> Consultar()
        {
            var lista = ConsultaBase()
                .OrderByDescending(x => x.FechaPrestamo)   // los mas recientes primero
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        public List<Prestamos> ConsultarPendientes()
        {
            var lista = ConsultaBase()
                .Where(x => !x.Devuelto)
                .OrderBy(x => x.FechaLimite)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        public List<Prestamos> ConsultarVencidos()
        {
            var ahora = DateTime.Now;
            var lista = ConsultaBase()
                .Where(x => !x.Devuelto && x.FechaLimite < ahora)
                .OrderBy(x => x.FechaLimite)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        public List<Prestamos> ConsultarPorLector(int lector)
        {
            var lista = ConsultaBase()
                .Where(x => x.Lector == lector)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        // PATRON: REUTILIZAR LA CONSULTA BASE
        // IQueryable = consulta "sin ejecutar": se le pueden seguir agregando Where/OrderBy
        // y solo va a la base cuando se llama ToList().
        private IQueryable<Prestamos> ConsultaBase()
        {
            return this.conexion.Prestamos!
                .Include(x => x._Libro)
                .Include(x => x._Lector);
        }

        private void LimpiarCiclos(List<Prestamos> lista)
        {
            foreach (var item in lista)
            {
                item._Libro!.Prestamos = null;
                item._Lector!.Prestamos = null;
            }
        }
    }
}
