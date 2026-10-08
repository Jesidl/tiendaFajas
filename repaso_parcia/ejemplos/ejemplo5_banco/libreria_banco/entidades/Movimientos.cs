using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_banco.entidades
{
    public class Movimientos
    {
        public int Id { get; set; }
        public int Cuenta { get; set; }             // FK -> Cuentas.Id
        public string? Tipo { get; set; }           // "Apertura" | "Deposito" | "Retiro"
        public decimal Valor { get; set; }
        public decimal Impuesto { get; set; }       // 4x1000 en retiros
        public decimal SaldoAnterior { get; set; }  // "foto" del saldo antes
        public decimal SaldoNuevo { get; set; }     // "foto" del saldo despues
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }

        [ForeignKey("Cuenta")] public Cuentas? _Cuenta { get; set; }
    }
}
