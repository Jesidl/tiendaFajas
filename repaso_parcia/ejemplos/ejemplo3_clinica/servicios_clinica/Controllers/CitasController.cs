using libreria_clinica.entidades;
using libreria_clinica.implementaciones;
using libreria_clinica.interfaces;
using libreria_clinica.nucleo;
using Microsoft.AspNetCore.Mvc;

namespace servicios_clinica.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitasController : ControllerBase
    {
        private ICitasAplicacion ICitasAplicacion;

        public CitasController()
        {
            var conexion = new Conexion();
            conexion.StringConexion = DatosGenerales.ObtenerStringConexion();

            this.ICitasAplicacion = new CitasAplicacion(conexion);
        }

        // GET /Citas
        [HttpGet]
        public IEnumerable<Citas> Get()
        {
            return this.ICitasAplicacion.Consultar();
        }

        // GET /Citas/PorMedico/1
        // {medico} en la ruta se pasa al parametro "int medico" (mismo nombre)
        [HttpGet("PorMedico/{medico}")]
        public IEnumerable<Citas> PorMedico(int medico)
        {
            return this.ICitasAplicacion.ConsultarPorMedico(medico);
        }

        // GET /Citas/Parcial -> crea paciente + medico + cita de prueba
        [HttpGet("Parcial")]
        public Citas Parcial()
        {
            var entidad = new Citas();
            // manana a las 9:00 am
            entidad.Fecha = DateTime.Today.AddDays(1).AddHours(9);

            entidad._Paciente = new Pacientes();
            entidad._Paciente.Documento = "P-" + DateTime.Now.Ticks;   // unico en cada llamada
            entidad._Paciente.Nombre = "Rosa Gomez";
            entidad._Paciente.FechaNacimiento = new DateTime(1955, 5, 20); // mayor de 60
            entidad._Paciente.Subsidiado = true;

            entidad._Medico = new Medicos();
            entidad._Medico.Nombre = "Dr. Perez";
            entidad._Medico.Especialidad = "Medicina general";
            entidad._Medico.Tarifa = 100000;

            // Esperado: 100000 -> subsidiado 50000 -> mayor 60 (-20%) = 40000
            return this.ICitasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Citas Insertar(Citas entidad)
        {
            return this.ICitasAplicacion.Insertar(entidad);
        }

        [HttpPut]
        public Citas Actualizar(Citas entidad)
        {
            return this.ICitasAplicacion.Actualizar(entidad);
        }

        // PUT /Citas/Atender/1?observacion=Paciente estable
        // "observacion" no esta en la ruta -> se toma de la query string (?observacion=...)
        [HttpPut("Atender/{id}")]
        public Citas Atender(int id, string? observacion)
        {
            return this.ICitasAplicacion.Atender(id, observacion);
        }

        // DELETE /Citas  body: { "id": 1 }  -> NO borra, CANCELA
        [HttpDelete]
        public Citas Borrar(Citas entidad)
        {
            return this.ICitasAplicacion.Borrar(entidad);
        }
    }
}
