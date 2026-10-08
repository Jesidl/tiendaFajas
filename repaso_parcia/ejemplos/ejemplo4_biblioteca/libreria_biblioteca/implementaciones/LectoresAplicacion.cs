using libreria_biblioteca.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_biblioteca.interfaces
{
    public class LectoresAplicacion : ILectoresAplicacion
    {
        private IConexion conexion;

        public LectoresAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Lectores Actualizar(Lectores entidad)
        {
            ValidarCorreo(entidad.Correo);

            var entry = this.conexion!.Entry<Lectores>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Lectores Borrar(Lectores entidad)
        {
            var lector = this.conexion.Lectores!.FirstOrDefault(x => x.Id == entidad.Id);
            if (lector == null)
                throw new Exception("El lector no existe");

            var debeLibros = this.conexion.Prestamos!
                .Any(x => x.Lector == lector.Id && !x.Devuelto);
            if (debeLibros)
                throw new Exception("El lector tiene libros sin devolver");

            // Borrado logico (se conserva el historial de prestamos)
            lector.Activo = false;
            var entry = this.conexion.Entry<Lectores>(lector);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return lector;
        }

        public List<Lectores> Consultar()
        {
            return this.conexion.Lectores!
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public Lectores Insertar(Lectores entidad)
        {
            ValidarCorreo(entidad.Correo);

            var repetido = this.conexion.Lectores!.Any(x => x.Documento == entidad.Documento);
            if (repetido)
                throw new Exception("Ya existe un lector con ese documento");

            entidad.Activo = true;

            this.conexion.Lectores!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // PATRON: VALIDAR FORMATO DE TEXTO (sin expresiones regulares)
        private void ValidarCorreo(string? correo)
        {
            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@") || !correo.Contains("."))
                throw new Exception("El correo no es valido");
        }
    }
}
