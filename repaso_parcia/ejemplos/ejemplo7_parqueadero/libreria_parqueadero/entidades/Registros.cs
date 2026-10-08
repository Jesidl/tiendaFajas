using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_parqueadero.entidades
{
    // Un registro = una estadia: entra (Insertar) y sale (Salida)
    public class Registros
    {
        public int Id { get; set; }
        public int Vehiculo { get; set; }          // FK -> Vehiculos.Id
        public int Celda { get; set; }             // FK -> Celdas.Id (0 = asignar automaticamente)
        public DateTime Entrada { get; set; }
        public DateTime? Salida { get; set; }      // null mientras el vehiculo esta adentro
        public int Minutos { get; set; }
        public decimal Valor { get; set; }
        public bool Cerrado { get; set; }

        [ForeignKey("Vehiculo")] public Vehiculos? _Vehiculo { get; set; }
        [ForeignKey("Celda")] public Celdas? _Celda { get; set; }
    }
}
