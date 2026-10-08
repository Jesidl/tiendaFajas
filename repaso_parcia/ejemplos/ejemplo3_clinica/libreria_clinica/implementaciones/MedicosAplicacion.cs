using libreria_clinica.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_clinica.interfaces
{
    public class MedicosAplicacion : IMedicosAplicacion
    {
        private IConexion conexion;

        public MedicosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Medicos Actualizar(Medicos entidad)
        {
            if (entidad.Tarifa <= 0)
                throw new Exception("La tarifa debe ser mayor a cero");

            var entry = this.conexion!.Entry<Medicos>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Medicos Borrar(Medicos entidad)
        {
            // PATRON: BORRADO FISICO CON RESTRICCION
            // Solo se deja borrar si NO tiene ninguna cita (ni siquiera historicas).
            var tieneCitas = this.conexion.Citas!.Any(x => x.Medico == entidad.Id);
            if (tieneCitas)
                throw new Exception("No se puede borrar un medico con citas registradas");

            this.conexion.Medicos!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Medicos> Consultar()
        {
            // PATRON: ORDENAR POR 2 CAMPOS -> OrderBy + ThenBy
            return this.conexion.Medicos!
                .Where(x => x.Activo)
                .OrderBy(x => x.Especialidad)
                .ThenBy(x => x.Nombre)
                .ToList();
        }

        public Medicos Insertar(Medicos entidad)
        {
            // PATRON: VALIDAR RANGO
            if (entidad.Tarifa <= 0)
                throw new Exception("La tarifa debe ser mayor a cero");

            entidad.Activo = true;

            this.conexion.Medicos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}
