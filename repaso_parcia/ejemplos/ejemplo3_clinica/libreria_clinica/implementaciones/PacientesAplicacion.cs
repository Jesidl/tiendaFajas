using libreria_clinica.entidades;
using Microsoft.EntityFrameworkCore;

// Nota: el profe pone las clases ...Aplicacion en el namespace "interfaces"
// aunque el archivo este en la carpeta implementaciones. Se respeta su estilo.
namespace libreria_clinica.interfaces
{
    public class PacientesAplicacion : IPacientesAplicacion
    {
        private IConexion conexion;

        public PacientesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Pacientes Actualizar(Pacientes entidad)
        {
            // PATRON: VALIDAR QUE EXISTA antes de actualizar
            var existe = this.conexion.Pacientes!.Any(x => x.Id == entidad.Id);
            if (!existe)
                throw new Exception("El paciente no existe");

            // PATRON: CAMPO UNICO al actualizar -> buscar otro registro (Id distinto)
            // con el mismo documento. Sin "x.Id != entidad.Id" siempre se encontraria a si mismo.
            var repetido = this.conexion.Pacientes!
                .Any(x => x.Documento == entidad.Documento && x.Id != entidad.Id);
            if (repetido)
                throw new Exception("Ya existe otro paciente con ese documento");

            var entry = this.conexion!.Entry<Pacientes>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Pacientes Borrar(Pacientes entidad)
        {
            // PATRON: BORRADO LOGICO (soft delete)
            // No se elimina la fila (se perderia el historial de citas por el Cascade);
            // se marca Activo = false y Consultar ya no lo muestra.
            var paciente = this.conexion.Pacientes!.FirstOrDefault(x => x.Id == entidad.Id);
            if (paciente == null)
                throw new Exception("El paciente no existe");

            // PATRON: NO PERMITIR si tiene registros "vivos" en la tabla hija
            var tieneCitasPendientes = this.conexion.Citas!
                .Any(x => x.Paciente == paciente.Id && x.Estado == "Programada");
            if (tieneCitasPendientes)
                throw new Exception("El paciente tiene citas programadas, cancelelas primero");

            paciente.Activo = false;

            // NoTracking -> hay que avisarle a EF que el objeto cambio
            var entry = this.conexion.Entry<Pacientes>(paciente);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return paciente;
        }

        public List<Pacientes> Consultar()
        {
            // PATRON: FILTRAR + ORDENAR
            return this.conexion.Pacientes!
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public Pacientes Insertar(Pacientes entidad)
        {
            // PATRON: CAMPO OBLIGATORIO
            if (string.IsNullOrWhiteSpace(entidad.Documento))
                throw new Exception("El documento es obligatorio");

            // PATRON: CAMPO UNICO al insertar -> Any() devuelve true si encuentra al menos uno
            var repetido = this.conexion.Pacientes!.Any(x => x.Documento == entidad.Documento);
            if (repetido)
                throw new Exception("Ya existe un paciente con ese documento");

            // PATRON: VALOR POR DEFECTO (lo pone el sistema, no el usuario)
            entidad.Activo = true;

            this.conexion.Pacientes!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}
