using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_banco.entidades
{
    // Esta tabla es HIJA de Clientes y PADRE de Movimientos:
    // tiene una FK (Cliente) y tambien una lista (Movimientos).
    public class Cuentas
    {
        public int Id { get; set; }
        public string? Numero { get; set; }         // lo GENERA la libreria: AH-000001
        public int Cliente { get; set; }            // FK -> Clientes.Id
        public string? Tipo { get; set; }           // "Ahorros" | "Corriente"
        public decimal Saldo { get; set; }          // PROTEGIDO: solo cambia con movimientos
        public DateTime FechaApertura { get; set; }
        public bool Activa { get; set; }

        [ForeignKey("Cliente")] public Clientes? _Cliente { get; set; }

        public List<Movimientos>? Movimientos { get; set; }
    }
}
