using libreria_universidad.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_universidad.interfaces
{
    public class CursosAplicacion : ICursosAplicacion
    {
        private IConexion conexion;

        public CursosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Cursos Actualizar(Cursos entidad)
        {
            Validar(entidad);

            // PATRON: NO DEJAR EL PADRE EN UN ESTADO INCONSISTENTE CON SUS HIJOS
            // No se puede bajar el cupo por debajo de los inscritos de ningun periodo.
            // GroupBy por periodo -> cuantos hay en cada uno -> el mayor.
            var inscritosPorPeriodo = this.conexion.Matriculas!
                .Where(x => x.Curso == entidad.Id && x.Estado != "Cancelada")
                .GroupBy(x => x.Periodo)
                .Select(g => g.Count())
                .ToList();
            int inscritos = inscritosPorPeriodo.Count > 0 ? inscritosPorPeriodo.Max() : 0;
            if (entidad.Cupo < inscritos)
                throw new Exception("El cupo no puede ser menor a los " + inscritos + " inscritos actuales");

            var entry = this.conexion!.Entry<Cursos>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Cursos Borrar(Cursos entidad)
        {
            var tieneMatriculas = this.conexion.Matriculas!.Any(x => x.Curso == entidad.Id);
            if (tieneMatriculas)
                throw new Exception("No se puede borrar un curso con matriculas");

            this.conexion.Cursos!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Cursos> Consultar()
        {
            return this.conexion.Cursos!
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public Cursos Insertar(Cursos entidad)
        {
            Validar(entidad);

            var repetido = this.conexion.Cursos!.Any(x => x.Codigo == entidad.Codigo);
            if (repetido)
                throw new Exception("Ya existe un curso con ese codigo");

            this.conexion.Cursos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // PATRON: VALIDACIONES AGRUPADAS en un metodo (se reusan en Insertar y Actualizar)
        private void Validar(Cursos entidad)
        {
            if (entidad.Creditos < 1 || entidad.Creditos > 5)
                throw new Exception("Los creditos deben estar entre 1 y 5");
            if (entidad.Cupo <= 0)
                throw new Exception("El cupo debe ser mayor a cero");
            if (entidad.ValorCredito <= 0)
                throw new Exception("El valor del credito debe ser mayor a cero");
        }
    }
}
