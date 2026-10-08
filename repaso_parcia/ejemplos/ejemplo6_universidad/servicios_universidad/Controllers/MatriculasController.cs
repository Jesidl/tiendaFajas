using libreria_universidad.entidades;
using libreria_universidad.implementaciones;
using libreria_universidad.interfaces;
using libreria_universidad.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_universidad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatriculasController : ControllerBase
    {
        private IMatriculasAplicacion IMatriculasAplicacion;

        public MatriculasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IMatriculasAplicacion = new MatriculasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Matriculas> Get()
        {
            return this.IMatriculasAplicacion.Consultar();
        }

        [HttpGet("PorEstudiante/{estudiante}")]
        public IEnumerable<Matriculas> PorEstudiante(int estudiante)
        {
            return this.IMatriculasAplicacion.ConsultarPorEstudiante(estudiante);
        }

        // GET /Matriculas/Resumen/2026-2
        [HttpGet("Resumen/{periodo}")]
        public IEnumerable<ResumenCurso> Resumen(string periodo)
        {
            return this.IMatriculasAplicacion.ResumenPorCurso(periodo);
        }

        // GET /Matriculas/Promedio/1
        [HttpGet("Promedio/{estudiante}")]
        public PromedioEstudiante Promedio(int estudiante)
        {
            return this.IMatriculasAplicacion.Promedio(estudiante);
        }

        // GET /Matriculas/Parcial -> estudiante estrato 2 + curso 3 creditos x 200000
        // Esperado: valor 600000, descuento 300000, total 300000
        [HttpGet("Parcial")]
        public Matriculas Parcial()
        {
            var entidad = new Matriculas();
            entidad.Periodo = "2026-2";

            entidad._Estudiante = new Estudiantes();
            entidad._Estudiante.Codigo = "E-" + DateTime.Now.Ticks;
            entidad._Estudiante.Nombre = "Camilo Rojas";
            entidad._Estudiante.Estrato = 2;

            entidad._Curso = new Cursos();
            entidad._Curso.Codigo = "CUR-" + DateTime.Now.Ticks;
            entidad._Curso.Nombre = "Bases de datos";
            entidad._Curso.Creditos = 3;
            entidad._Curso.Cupo = 2;
            entidad._Curso.ValorCredito = 200000;

            return this.IMatriculasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Matriculas Insertar(Matriculas entidad)
        {
            return this.IMatriculasAplicacion.Insertar(entidad);
        }

        // PUT /Matriculas/Calificar/1/4.5  (decimal con PUNTO)
        [HttpPut("Calificar/{id}/{nota}")]
        public Matriculas Calificar(int id, decimal nota)
        {
            return this.IMatriculasAplicacion.Calificar(id, nota);
        }

        // PUT /Matriculas  { "id": 1, "curso": 2 } -> cambio de curso
        [HttpPut]
        public Matriculas Actualizar(Matriculas entidad)
        {
            return this.IMatriculasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Matriculas Borrar(Matriculas entidad)
        {
            return this.IMatriculasAplicacion.Borrar(entidad);
        }
    }
}
