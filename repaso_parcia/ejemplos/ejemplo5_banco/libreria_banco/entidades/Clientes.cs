namespace libreria_banco.entidades
{
    // RELACION EN CADENA:  Clientes 1 --- N Cuentas 1 --- N Movimientos
    // (distinto a tienda/notas donde la tabla del medio tenia 2 FK)
    public class Clientes
    {
        public int Id { get; set; }
        public string? Documento { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; }

        public List<Cuentas>? Cuentas { get; set; }
    }
}
