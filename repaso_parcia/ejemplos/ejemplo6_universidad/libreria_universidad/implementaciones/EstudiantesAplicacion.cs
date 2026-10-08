using libreria_universidad.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_universidad.interfaces
{
    public class EstudiantesAplicacion : IEstudiantesAplicacion
    {
        private IConexion conexion;

        public EstudiantesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Estudiantes Actualizar(Estudiantes entidad)
        {
            ValidarEstrato(entidad.Estrato);

            var entry = this.conexion!.Entry<Estudiantes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Estudiantes Borrar(Estudiantes entidad)
        {
            var estudiante = this.conexion.Estudiantes!.FirstOrDefault(x => x.Id == entidad.Id);
            if (estudiante == null)
                throw new Exception("El estudiante no existe");

            var matriculado = this.conexion.Matriculas!
                .Any(x => x.Estudiante == estudiante.Id && x.Estado == "Activa");
            if (matriculado)
                throw new Exception("El estudiante tiene matriculas activas");

            estudiante.Activo = false;
            var entry = this.conexion.Entry<Estudiantes>(estudiante);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return estudiante;
        }

        public List<Estudiantes> Consultar()
        {
            return this.conexion.Estudiantes!
                .Where(x => x.Activo)
                .OrderBy(x => x.Codigo)
                .ToList();
        }

        public Estudiantes Insertar(Estudiantes entidad)
        {
            ValidarEstrato(entidad.Estrato);

            var repetido = this.conexion.Estudiantes!.Any(x => x.Codigo == entidad.Codigo);
            if (repetido)
                throw new Exception("Ya existe un estudiante con ese codigo");

            entidad.Activo = true;

            this.conexion.Estudiantes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // PATRON: VALIDAR RANGO NUMERICO
        private void ValidarEstrato(int estrato)
        {
            if (estrato < 1 || estrato > 6)
                throw new Exception("El estrato debe estar entre 1 y 6");
        }
    }
}
