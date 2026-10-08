using libreria_notas.entidades;
using libreria_notas.implementaciones;
using libreria_notas.interfaces;
using libreria_notas.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_notas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotasController : ControllerBase
    {
        private INotasAplicacion INotasAplicacion;

        public NotasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.INotasAplicacion = new NotasAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Notas> Get()
        {
            return this.INotasAplicacion.Consultar();
        }

        // Prueba rapida desde el navegador: http://localhost:5029/Notas/Parcial
        // Crea un estudiante y una nota de ejemplo y los guarda.
        [HttpGet("Parcial")]
        public Notas Parcial()
        {
            var entidad = new Notas();
            entidad.Materia = "Programacion";
            entidad.Nota1 = 5;
            entidad.Nota2 = 4;
            entidad.Nota3 = 3;
            entidad.Nota4 = 4;
            entidad.Nota5 = 5;
            entidad.NotaFinal = 4.2m;

            entidad._Estudiante = new Estudiantes();
            entidad._Estudiante.Codigo = "123";
            entidad._Estudiante.Nombres = "Pepito";
            entidad._Estudiante.Fecha = DateTime.Now;
            entidad._Estudiante.Estado = true;

            return this.INotasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Notas Insertar(Notas entidad)
        {
            return this.INotasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Notas Actualizar(Notas entidad)
        {
            return this.INotasAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Notas Borrar(Notas entidad)
        {
            return this.INotasAplicacion.Borrar(entidad);
        }
    }
}
