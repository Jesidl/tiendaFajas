using libreria_biblioteca.entidades;
using libreria_biblioteca.implementaciones;
using libreria_biblioteca.interfaces;
using libreria_biblioteca.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrestamosController : ControllerBase
    {
        private IPrestamosAplicacion IPrestamosAplicacion;

        public PrestamosController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.IPrestamosAplicacion = new PrestamosAplicacion(conexion);
        }

        [HttpGet]
        public IEnumerable<Prestamos> Get()
        {
            return this.IPrestamosAplicacion.Consultar();
        }

        // Cada GET extra necesita su PROPIA ruta (si no -> AmbiguousMatchException)
        [HttpGet("Pendientes")]
        public IEnumerable<Prestamos> Pendientes()
        {
            return this.IPrestamosAplicacion.ConsultarPendientes();
        }

        [HttpGet("Vencidos")]
        public IEnumerable<Prestamos> Vencidos()
        {
            return this.IPrestamosAplicacion.ConsultarVencidos();
        }

        [HttpGet("PorLector/{lector}")]
        public IEnumerable<Prestamos> PorLector(int lector)
        {
            return this.IPrestamosAplicacion.ConsultarPorLector(lector);
        }

        // GET /Prestamos/Parcial -> lector + libro nuevos, prestado hace 20 dias (ya vencido)
        [HttpGet("Parcial")]
        public Prestamos Parcial()
        {
            var entidad = new Prestamos();
            entidad.FechaPrestamo = DateTime.Now.AddDays(-20);   // limite = hace 5 dias

            entidad._Lector = new Lectores();
            entidad._Lector.Documento = "L-" + DateTime.Now.Ticks;
            entidad._Lector.Nombre = "Ana Torres";
            entidad._Lector.Correo = "ana@correo.com";

            entidad._Libro = new Libros();
            entidad._Libro.Isbn = "ISBN-" + DateTime.Now.Ticks;
            entidad._Libro.Titulo = "Cien anos de soledad";
            entidad._Libro.Autor = "Gabriel Garcia Marquez";
            entidad._Libro.Ejemplares = 3;                      // quedan 2 disponibles

            return this.IPrestamosAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Prestamos Insertar(Prestamos entidad)
        {
            return this.IPrestamosAplicacion.Insertar(entidad);
        }

        // PUT /Prestamos/Devolver/1 -> sin body, el id va en la URL
        [HttpPut("Devolver/{id}")]
        public Prestamos Devolver(int id)
        {
            return this.IPrestamosAplicacion.Devolver(id);
        }

        // PUT /Prestamos -> renovar (solo cambia fechaLimite)
        [HttpPut]
        public Prestamos Actualizar(Prestamos entidad)
        {
            return this.IPrestamosAplicacion.Actualizar(entidad);
        }

        [HttpDelete]
        public Prestamos Borrar(Prestamos entidad)
        {
            return this.IPrestamosAplicacion.Borrar(entidad);
        }
    }
}
