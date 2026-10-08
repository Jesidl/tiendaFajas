using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_clinica.entidades
{
    // Tabla detalle: tiene las 2 llaves foraneas (igual que Notas / Ventas)
    public class Citas
    {
        public int Id { get; set; }
        public int Paciente { get; set; }      // FK -> Pacientes.Id
        public int Medico { get; set; }        // FK -> Medicos.Id
        public DateTime Fecha { get; set; }    // fecha y HORA de la cita
        public string? Estado { get; set; }    // "Programada" | "Atendida" | "Cancelada"
        public decimal Valor { get; set; }     // lo CALCULA la libreria, no el usuario
        public string? Observacion { get; set; }

        [ForeignKey("Paciente")] public Pacientes? _Paciente { get; set; }
        [ForeignKey("Medico")] public Medicos? _Medico { get; set; }
    }
}
