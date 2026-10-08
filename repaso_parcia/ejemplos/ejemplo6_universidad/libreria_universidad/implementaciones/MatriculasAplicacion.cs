using libreria_universidad.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_universidad.interfaces
{
    public class MatriculasAplicacion : IMatriculasAplicacion
    {
        private const int MAX_CREDITOS_PERIODO = 20;
        private const decimal NOTA_APROBATORIA = 3.0m;

        private IConexion conexion;

        public MatriculasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR = MATRICULAR
        // =====================================================================
        public Matriculas Insertar(Matriculas entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Periodo))
                throw new Exception("El periodo es obligatorio");

            // Valores del sistema
            entidad.Fecha = DateTime.Now;
            entidad.Estado = "Activa";
            entidad.Nota = null;

            // ---- Estudiante ----
            Estudiantes? estudiante;
            if (entidad._Estudiante != null)
            {
                estudiante = entidad._Estudiante;
                estudiante.Activo = true;
            }
            else
            {
                estudiante = this.conexion.Estudiantes!.FirstOrDefault(x => x.Id == entidad.Estudiante);
                if (estudiante == null || !estudiante.Activo)
                    throw new Exception("El estudiante no existe o esta inactivo");
            }

            // ---- Curso ----
            var curso = entidad._Curso
                ?? this.conexion.Cursos!.FirstOrDefault(x => x.Id == entidad.Curso);
            if (curso == null)
                throw new Exception("El curso no existe");

            // ---- Reglas contra la BD (solo si el curso ya existe; uno nuevo no tiene inscritos) ----
            if (entidad._Curso == null)
                ValidarReglasDeCupo(entidad, curso);

            // ---- Calculos ----
            CalcularValores(entidad, curso, estudiante);

            // ---- Guardar ----
            if (entidad._Estudiante != null)
                this.conexion.Estudiantes!.Add(entidad._Estudiante);
            if (entidad._Curso != null)
                this.conexion.Cursos!.Add(entidad._Curso);

            this.conexion.Matriculas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // ACTUALIZAR = CAMBIAR DE CURSO dentro del mismo periodo
        // PATRON: RE-VALIDAR EXCLUYENDO EL PROPIO REGISTRO
        // =====================================================================
        public Matriculas Actualizar(Matriculas entidad)
        {
            var original = this.conexion.Matriculas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("La matricula no existe");
            if (original.Estado != "Activa")
                throw new Exception("Solo se pueden modificar matriculas activas");

            var curso = this.conexion.Cursos!.FirstOrDefault(x => x.Id == entidad.Curso);
            if (curso == null)
                throw new Exception("El curso no existe");
            var estudiante = this.conexion.Estudiantes!.FirstOrDefault(x => x.Id == original.Estudiante);

            // Solo es editable el curso; lo demas se toma del original
            original.Curso = entidad.Curso;

            ValidarReglasDeCupo(original, curso);       // usa original.Id para excluirse
            CalcularValores(original, curso, estudiante!);

            var entry = this.conexion.Entry<Matriculas>(original);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return original;
        }

        // BORRAR = CANCELAR (solo si todavia no tiene nota)
        public Matriculas Borrar(Matriculas entidad)
        {
            var matricula = this.conexion.Matriculas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (matricula == null)
                throw new Exception("La matricula no existe");
            if (matricula.Estado != "Activa")
                throw new Exception("Solo se pueden cancelar matriculas activas (estado actual: " + matricula.Estado + ")");

            matricula.Estado = "Cancelada";
            var entry = this.conexion.Entry<Matriculas>(matricula);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return matricula;
        }

        // =====================================================================
        // CALIFICAR: accion que cambia el estado segun un calculo
        // =====================================================================
        public Matriculas Calificar(int id, decimal nota)
        {
            if (nota < 0 || nota > 5)
                throw new Exception("La nota debe estar entre 0 y 5");

            var matricula = this.conexion.Matriculas!.FirstOrDefault(x => x.Id == id);
            if (matricula == null)
                throw new Exception("La matricula no existe");
            if (matricula.Estado != "Activa")
                throw new Exception("Solo se califican matriculas activas");

            matricula.Nota = nota;
            // PATRON: ESTADO DERIVADO DE UN VALOR (operador ternario)
            matricula.Estado = nota >= NOTA_APROBATORIA ? "Aprobada" : "Reprobada";

            var entry = this.conexion.Entry<Matriculas>(matricula);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return matricula;
        }

        public List<Matriculas> Consultar()
        {
            var lista = this.conexion.Matriculas!
                .Include(x => x._Estudiante)
                .Include(x => x._Curso)
                .OrderBy(x => x.Periodo)
                .ThenBy(x => x._Estudiante!.Nombre)    // se puede ordenar por un campo del Include
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        public List<Matriculas> ConsultarPorEstudiante(int estudiante)
        {
            var lista = this.conexion.Matriculas!
                .Include(x => x._Estudiante)
                .Include(x => x._Curso)
                .Where(x => x.Estudiante == estudiante)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        // =====================================================================
        // REPORTE 1: resumen por curso -> GroupBy
        // =====================================================================
        public List<ResumenCurso> ResumenPorCurso(string periodo)
        {
            var matriculas = this.conexion.Matriculas!
                .Include(x => x._Curso)
                .Where(x => x.Periodo == periodo && x.Estado != "Cancelada")
                .ToList();                                   // a memoria

            var resultado = new List<ResumenCurso>();

            // GroupBy: arma "grupos" de matriculas que tienen el mismo curso.
            // grupo.Key = el valor por el que se agrupo (el Id del curso)
            foreach (var grupo in matriculas.GroupBy(x => x.Curso))
            {
                var curso = grupo.First()._Curso!;
                var calificadas = grupo.Where(x => x.Nota != null).ToList();

                var resumen = new ResumenCurso();
                resumen.Curso = curso.Nombre;
                resumen.Cupo = curso.Cupo;
                resumen.Inscritos = grupo.Count();
                resumen.CuposDisponibles = curso.Cupo - grupo.Count();   // el grupo ya excluye canceladas
                resumen.Recaudo = grupo.Sum(x => x.Total);
                // Average falla si la lista esta vacia -> se pregunta antes con Any/Count
                resumen.Promedio = calificadas.Count > 0
                    ? Math.Round(calificadas.Average(x => x.Nota!.Value), 2)
                    : null;
                resultado.Add(resumen);
            }

            return resultado.OrderByDescending(x => x.Inscritos).ToList();
        }

        // =====================================================================
        // REPORTE 2: PROMEDIO PONDERADO por creditos
        //   promedio = suma(nota * creditos) / suma(creditos)
        // =====================================================================
        public PromedioEstudiante Promedio(int estudiante)
        {
            var datos = this.conexion.Estudiantes!.FirstOrDefault(x => x.Id == estudiante);
            if (datos == null)
                throw new Exception("El estudiante no existe");

            var calificadas = this.conexion.Matriculas!
                .Include(x => x._Curso)
                .Where(x => x.Estudiante == estudiante && x.Nota != null)
                .ToList();

            var resultado = new PromedioEstudiante();
            resultado.Estudiante = datos.Nombre;
            resultado.CursosCalificados = calificadas.Count;
            resultado.CreditosAprobados = calificadas
                .Where(x => x.Estado == "Aprobada")
                .Sum(x => x._Curso!.Creditos);

            int totalCreditos = calificadas.Sum(x => x._Curso!.Creditos);
            if (totalCreditos > 0)       // evitar division por cero
            {
                decimal sumaPonderada = calificadas.Sum(x => x.Nota!.Value * x._Curso!.Creditos);
                resultado.PromedioPonderado = Math.Round(sumaPonderada / totalCreditos, 2);
            }
            return resultado;
        }

        // ------------------------------ ayudas ------------------------------

        // Reglas de matricula que consultan la BD:
        //  1. no repetir curso en el mismo periodo
        //  2. no superar el cupo del curso
        //  3. no superar 20 creditos en el periodo
        private void ValidarReglasDeCupo(Matriculas entidad, Cursos curso)
        {
            // 1. DUPLICADO (combinacion de 3 campos)
            var duplicada = this.conexion.Matriculas!.Any(x =>
                x.Estudiante == entidad.Estudiante &&
                x.Curso == entidad.Curso &&
                x.Periodo == entidad.Periodo &&
                x.Estado != "Cancelada" &&
                x.Id != entidad.Id);
            if (duplicada)
                throw new Exception("El estudiante ya esta matriculado en ese curso para el periodo");

            // 2. CUPO -> Count con condicion
            //    Ocupa cupo toda matricula NO cancelada del periodo
            //    (las Aprobadas/Reprobadas tambien fueron parte del grupo).
            int inscritos = this.conexion.Matriculas!.Count(x =>
                x.Curso == entidad.Curso &&
                x.Periodo == entidad.Periodo &&
                x.Estado != "Cancelada" &&
                x.Id != entidad.Id);
            if (inscritos >= curso.Cupo)
                throw new Exception("El curso " + curso.Nombre + " no tiene cupos");

            // 3. LIMITE ACUMULADO -> Sum sobre un campo de OTRA tabla (x._Curso.Creditos)
            //    EF lo traduce a un JOIN automaticamente.
            int creditosActuales = this.conexion.Matriculas!
                .Where(x => x.Estudiante == entidad.Estudiante &&
                            x.Periodo == entidad.Periodo &&
                            x.Estado != "Cancelada" &&
                            x.Id != entidad.Id)
                .Sum(x => x._Curso!.Creditos);
            if (creditosActuales + curso.Creditos > MAX_CREDITOS_PERIODO)
                throw new Exception("Supera el maximo de " + MAX_CREDITOS_PERIODO + " creditos. Ya tiene " + creditosActuales);
        }

        // PATRON: DESCUENTO POR CATEGORIA -> switch
        private void CalcularValores(Matriculas entidad, Cursos curso, Estudiantes estudiante)
        {
            entidad.Valor = curso.Creditos * curso.ValorCredito;

            decimal porcentaje;
            switch (estudiante.Estrato)
            {
                case 1:
                case 2:
                    porcentaje = 0.50m;     // estratos 1 y 2: 50%
                    break;
                case 3:
                    porcentaje = 0.25m;     // estrato 3: 25%
                    break;
                default:
                    porcentaje = 0m;        // 4, 5, 6: sin descuento
                    break;
            }

            entidad.Descuento = Math.Round(entidad.Valor * porcentaje, 2);
            entidad.Total = entidad.Valor - entidad.Descuento;
        }

        private void LimpiarCiclos(List<Matriculas> lista)
        {
            foreach (var item in lista)
            {
                item._Estudiante!.Matriculas = null;
                item._Curso!.Matriculas = null;
            }
        }
    }
}
