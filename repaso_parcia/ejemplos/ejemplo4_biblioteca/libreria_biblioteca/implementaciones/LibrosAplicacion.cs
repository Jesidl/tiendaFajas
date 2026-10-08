using libreria_biblioteca.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_biblioteca.interfaces
{
    public class LibrosAplicacion : ILibrosAplicacion
    {
        private IConexion conexion;

        public LibrosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Libros Actualizar(Libros entidad)
        {
            // PATRON: RECALCULAR UN CAMPO DERIVADO A PARTIR DEL ORIGINAL
            // Si cambian el total de ejemplares, los disponibles deben ajustarse
            // respetando los que estan prestados en este momento.
            var original = this.conexion.Libros!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("El libro no existe");

            int prestados = original.Ejemplares - original.Disponibles;

            if (entidad.Ejemplares < prestados)
                throw new Exception("No puede tener menos ejemplares que los " + prestados + " que estan prestados");

            entidad.Disponibles = entidad.Ejemplares - prestados;

            var entry = this.conexion!.Entry<Libros>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Libros Borrar(Libros entidad)
        {
            // PATRON: NO BORRAR SI TIENE HIJOS "ACTIVOS"
            var prestado = this.conexion.Prestamos!
                .Any(x => x.Libro == entidad.Id && !x.Devuelto);
            if (prestado)
                throw new Exception("El libro tiene prestamos sin devolver");

            this.conexion.Libros!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Libros> Consultar()
        {
            return this.conexion.Libros!
                .OrderBy(x => x.Titulo)
                .ToList();
        }

        public List<Libros> ConsultarDisponibles()
        {
            return this.conexion.Libros!
                .Where(x => x.Disponibles > 0)
                .OrderBy(x => x.Titulo)
                .ToList();
        }

        public Libros Insertar(Libros entidad)
        {
            if (entidad.Ejemplares <= 0)
                throw new Exception("Debe registrar al menos un ejemplar");

            var repetido = this.conexion.Libros!.Any(x => x.Isbn == entidad.Isbn);
            if (repetido)
                throw new Exception("Ya existe un libro con ese ISBN");

            // PATRON: CAMPO DERIVADO al crear -> al inicio todos los ejemplares estan disponibles
            entidad.Disponibles = entidad.Ejemplares;

            this.conexion.Libros!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}
