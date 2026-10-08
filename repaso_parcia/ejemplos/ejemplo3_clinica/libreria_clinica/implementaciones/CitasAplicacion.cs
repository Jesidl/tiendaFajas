using libreria_clinica.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_clinica.interfaces
{
    public class CitasAplicacion : ICitasAplicacion
    {
        private IConexion conexion;

        public CitasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR: el metodo con mas reglas. Orden SIEMPRE:
        //   1. valores por defecto  2. validaciones simples
        //   3. traer datos relacionados  4. validaciones contra la BD
        //   5. calculos  6. Add de relacionados nuevos  7. Add + SaveChanges
        // =====================================================================
        public Citas Insertar(Citas entidad)
        {
            // ---- 1. VALORES POR DEFECTO ----
            // El estado NO lo decide el usuario: toda cita nueva nace "Programada".
            entidad.Estado = "Programada";
            // Si vienen paciente/medico NUEVOS dentro de la cita, nacen activos
            if (entidad._Paciente != null)
                entidad._Paciente.Activo = true;
            if (entidad._Medico != null)
                entidad._Medico.Activo = true;

            // ---- 2. VALIDACIONES SIMPLES (sin ir a la BD) ----
            if (entidad.Fecha <= DateTime.Now)
                throw new Exception("La cita debe ser en una fecha futura");

            // ---- 3. TRAER DATOS RELACIONADOS ----
            // Operador ?? : si la cita trae el objeto completo (caso Parcial) se usa ese,
            // si no, se busca en la base por el Id de la llave foranea.
            var paciente = entidad._Paciente
                ?? this.conexion.Pacientes!.FirstOrDefault(x => x.Id == entidad.Paciente);
            if (paciente == null)
                throw new Exception("El paciente no existe");
            if (!paciente.Activo)
                throw new Exception("El paciente esta inactivo");

            var medico = entidad._Medico
                ?? this.conexion.Medicos!.FirstOrDefault(x => x.Id == entidad.Medico);
            if (medico == null)
                throw new Exception("El medico no existe");
            if (!medico.Activo)
                throw new Exception("El medico esta inactivo");

            // ---- 4. VALIDACIONES CONTRA LA BD ----
            // Solo tienen sentido si el medico/paciente YA existen (si son nuevos no tienen citas).
            if (entidad._Medico == null)
                ValidarChoqueHorario(entidad);

            if (entidad._Paciente == null)
            {
                // PATRON: RANGO DE FECHAS -> "el mismo dia" = desde las 00:00 hasta las 00:00 del dia siguiente.
                // Las variables se calculan ANTES de la consulta (EF traduce la consulta a SQL).
                var inicioDia = entidad.Fecha.Date;
                var finDia = inicioDia.AddDays(1);
                var yaTieneCita = this.conexion.Citas!.Any(x =>
                    x.Paciente == entidad.Paciente &&
                    x.Estado != "Cancelada" &&
                    x.Fecha >= inicioDia && x.Fecha < finDia);
                if (yaTieneCita)
                    throw new Exception("El paciente ya tiene una cita ese dia");
            }

            // ---- 5. CALCULOS ----
            entidad.Valor = CalcularValor(paciente, medico);

            // ---- 6. RELACIONADOS NUEVOS (igual que examen.txt) ----
            if (entidad._Paciente != null)
                this.conexion.Pacientes!.Add(entidad._Paciente);
            if (entidad._Medico != null)
                this.conexion.Medicos!.Add(entidad._Medico);

            // ---- 7. GUARDAR (un solo SaveChanges = todo o nada) ----
            this.conexion.Citas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // ACTUALIZAR = reprogramar la cita
        // PATRON: CARGAR EL ORIGINAL para comparar / proteger campos
        // =====================================================================
        public Citas Actualizar(Citas entidad)
        {
            var original = this.conexion.Citas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("La cita no existe");

            // Regla de estado: solo se modifican citas programadas
            if (original.Estado != "Programada")
                throw new Exception("Solo se pueden modificar citas programadas");

            if (entidad.Fecha <= DateTime.Now)
                throw new Exception("La cita debe ser en una fecha futura");

            ValidarChoqueHorario(entidad);

            // PATRON: PROTEGER CAMPOS -> el usuario no puede cambiar el estado por PUT,
            // se conserva el que estaba en la base.
            entidad.Estado = original.Estado;

            // Recalcular: si cambiaron de medico, cambia la tarifa
            var paciente = this.conexion.Pacientes!.FirstOrDefault(x => x.Id == entidad.Paciente);
            var medico = this.conexion.Medicos!.FirstOrDefault(x => x.Id == entidad.Medico);
            if (paciente == null || medico == null)
                throw new Exception("Paciente o medico no existe");
            entidad.Valor = CalcularValor(paciente, medico);

            // Importante: no mandar objetos de navegacion al marcar Modified
            entidad._Paciente = null;
            entidad._Medico = null;

            var entry = this.conexion!.Entry<Citas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // BORRAR = CANCELAR (borrado logico cambiando el ESTADO)
        // =====================================================================
        public Citas Borrar(Citas entidad)
        {
            // Del JSON solo nos importa el Id: el resto se carga de la base
            var cita = this.conexion.Citas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (cita == null)
                throw new Exception("La cita no existe");
            if (cita.Estado == "Atendida")
                throw new Exception("No se puede cancelar una cita ya atendida");
            if (cita.Estado == "Cancelada")
                throw new Exception("La cita ya estaba cancelada");

            cita.Estado = "Cancelada";

            var entry = this.conexion.Entry<Citas>(cita);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return cita;
        }

        public List<Citas> Consultar()
        {
            var lista = this.conexion.Citas!
                .Include(x => x._Paciente)
                .Include(x => x._Medico)
                .OrderBy(x => x.Fecha)
                .ToList();

            LimpiarCiclos(lista);
            return lista;
        }

        // PATRON: CONSULTA CON PARAMETRO (endpoint /Citas/PorMedico/1)
        public List<Citas> ConsultarPorMedico(int medico)
        {
            var lista = this.conexion.Citas!
                .Include(x => x._Paciente)
                .Include(x => x._Medico)
                .Where(x => x.Medico == medico && x.Estado != "Cancelada")
                .OrderBy(x => x.Fecha)
                .ToList();

            LimpiarCiclos(lista);
            return lista;
        }

        // PATRON: ACCION DE NEGOCIO (cambio de estado) que no es ni insertar ni borrar
        public Citas Atender(int id, string? observacion)
        {
            var cita = this.conexion.Citas!.FirstOrDefault(x => x.Id == id);
            if (cita == null)
                throw new Exception("La cita no existe");
            if (cita.Estado != "Programada")
                throw new Exception("Solo se pueden atender citas programadas");

            cita.Estado = "Atendida";
            cita.Observacion = observacion;

            var entry = this.conexion.Entry<Citas>(cita);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return cita;
        }

        // ------------------------- METODOS PRIVADOS (ayudas) -------------------------

        // PATRON: CHOQUE DE HORARIO
        // Cada cita dura 30 min -> hay choque si otra cita del mismo medico
        // empieza menos de 30 min antes o despues.
        private void ValidarChoqueHorario(Citas entidad)
        {
            var desde = entidad.Fecha.AddMinutes(-30);
            var hasta = entidad.Fecha.AddMinutes(30);

            var choca = this.conexion.Citas!.Any(x =>
                x.Medico == entidad.Medico &&
                x.Id != entidad.Id &&                // al actualizar, no compararse consigo misma
                x.Estado != "Cancelada" &&           // las canceladas liberan el horario
                x.Fecha > desde && x.Fecha < hasta);

            if (choca)
                throw new Exception("El medico ya tiene una cita en ese horario");
        }

        // PATRON: CALCULO CON VARIAS REGLAS ENCADENADAS
        //  - base = tarifa del medico
        //  - subsidiado -> 50% de descuento
        //  - 60 anos o mas -> 20% adicional sobre lo que quede
        private decimal CalcularValor(Pacientes paciente, Medicos medico)
        {
            decimal valor = medico.Tarifa;

            if (paciente.Subsidiado)
                valor = valor * 0.5m;            // la "m" indica que el numero es decimal

            if (CalcularEdad(paciente.FechaNacimiento) >= 60)
                valor = valor - (valor * 0.20m);

            return Math.Round(valor, 2);
        }

        // PATRON: EDAD a partir de fecha de nacimiento
        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;
            // si todavia no ha cumplido este ano, se resta 1
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;
            return edad;
        }

        // Corta el ciclo cita -> paciente -> citas -> cita ... para el JSON
        private void LimpiarCiclos(List<Citas> lista)
        {
            foreach (var item in lista)
            {
                item._Paciente!.Citas = null;
                item._Medico!.Citas = null;
            }
        }
    }
}
