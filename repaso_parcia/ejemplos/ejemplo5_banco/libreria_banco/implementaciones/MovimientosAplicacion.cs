using libreria_banco.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_banco.interfaces
{
    public class MovimientosAplicacion : IMovimientosAplicacion
    {
        private const decimal GMF = 0.004m;                       // 4 x 1000
        private const decimal LIMITE_RETIRO_DIARIO = 2000000m;

        private IConexion conexion;

        public MovimientosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR = DEPOSITAR o RETIRAR
        // PATRON: ACTUALIZAR UN ACUMULADO EN LA TABLA PADRE (saldo)
        // =====================================================================
        public Movimientos Insertar(Movimientos entidad)
        {
            // ---- 1. Validaciones simples ----
            if (entidad.Tipo != "Deposito" && entidad.Tipo != "Retiro")
                throw new Exception("El tipo debe ser Deposito o Retiro");
            if (entidad.Valor <= 0)
                throw new Exception("El valor debe ser mayor a cero");

            // ---- 2. Traer la cuenta (tabla padre) ----
            var cuenta = this.conexion.Cuentas!.FirstOrDefault(x => x.Id == entidad.Cuenta);
            if (cuenta == null)
                throw new Exception("La cuenta no existe");
            if (!cuenta.Activa)
                throw new Exception("La cuenta esta bloqueada");

            // ---- 3. Calculos segun el tipo ----
            entidad.Fecha = DateTime.Now;
            entidad.SaldoAnterior = cuenta.Saldo;

            if (entidad.Tipo == "Deposito")
            {
                entidad.Impuesto = 0;
                entidad.SaldoNuevo = cuenta.Saldo + entidad.Valor;
            }
            else // Retiro
            {
                // PATRON: IMPUESTO / PORCENTAJE + REDONDEO
                entidad.Impuesto = Math.Round(entidad.Valor * GMF, 2);
                decimal totalDebitar = entidad.Valor + entidad.Impuesto;

                // PATRON: NO PERMITIR SALDO NEGATIVO
                if (totalDebitar > cuenta.Saldo)
                    throw new Exception("Saldo insuficiente. Disponible: " + cuenta.Saldo + ", requerido: " + totalDebitar);

                // PATRON: LIMITE ACUMULADO POR PERIODO -> Sum() de lo del dia
                var hoy = DateTime.Today;
                var manana = hoy.AddDays(1);
                decimal retiradoHoy = this.conexion.Movimientos!
                    .Where(x => x.Cuenta == cuenta.Id && x.Tipo == "Retiro" && x.Fecha >= hoy && x.Fecha < manana)
                    .Sum(x => x.Valor);
                if (retiradoHoy + entidad.Valor > LIMITE_RETIRO_DIARIO)
                    throw new Exception("Supera el limite diario de retiro. Ya retiro hoy: " + retiradoHoy);

                entidad.SaldoNuevo = cuenta.Saldo - totalDebitar;
            }

            // ---- 4. Actualizar la tabla padre ----
            cuenta.Saldo = entidad.SaldoNuevo;
            var entryCuenta = this.conexion.Entry<Cuentas>(cuenta);
            entryCuenta.State = EntityState.Modified;

            // ---- 5. Guardar todo junto ----
            entidad._Cuenta = null;
            this.conexion.Movimientos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // PATRON: OPERACION PROHIBIDA
        // En contabilidad un movimiento no se edita ni se borra: se hace otro que lo compense.
        public Movimientos Actualizar(Movimientos entidad)
        {
            throw new Exception("Los movimientos no se pueden modificar. Registre un movimiento contrario");
        }

        public Movimientos Borrar(Movimientos entidad)
        {
            throw new Exception("Los movimientos no se pueden borrar. Registre un movimiento contrario");
        }

        public List<Movimientos> Consultar()
        {
            // PATRON: INCLUDE EN CADENA -> ThenInclude
            // movimiento -> su cuenta -> el cliente de esa cuenta
            var lista = this.conexion.Movimientos!
                .Include(x => x._Cuenta)
                    .ThenInclude(c => c!._Cliente)
                .OrderByDescending(x => x.Fecha)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        public List<Movimientos> ConsultarPorCuenta(int cuenta)
        {
            var lista = this.conexion.Movimientos!
                .Include(x => x._Cuenta)
                    .ThenInclude(c => c!._Cliente)
                .Where(x => x.Cuenta == cuenta)
                .OrderBy(x => x.Fecha)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        // =====================================================================
        // EXTRACTO: REPORTE que devuelve un DTO (no se guarda nada)
        // =====================================================================
        public Extracto Extracto(int cuenta)
        {
            var datosCuenta = this.conexion.Cuentas!
                .Include(x => x._Cliente)
                .FirstOrDefault(x => x.Id == cuenta);
            if (datosCuenta == null)
                throw new Exception("La cuenta no existe");

            // Se traen los movimientos UNA vez y se calcula en memoria (LINQ sobre la lista)
            var movimientos = this.conexion.Movimientos!
                .Where(x => x.Cuenta == cuenta)
                .OrderBy(x => x.Fecha)
                .ToList();

            var extracto = new Extracto();
            extracto.Numero = datosCuenta.Numero;
            extracto.Titular = datosCuenta._Cliente!.Nombre;
            extracto.Tipo = datosCuenta.Tipo;
            extracto.SaldoActual = datosCuenta.Saldo;
            // Sum con condicion: primero Where, luego Sum
            extracto.TotalDepositos = movimientos.Where(x => x.Tipo != "Retiro").Sum(x => x.Valor);
            extracto.TotalRetiros = movimientos.Where(x => x.Tipo == "Retiro").Sum(x => x.Valor);
            extracto.TotalImpuestos = movimientos.Sum(x => x.Impuesto);
            extracto.CantidadMovimientos = movimientos.Count;
            extracto.Movimientos = movimientos;
            return extracto;
        }

        private void LimpiarCiclos(List<Movimientos> lista)
        {
            foreach (var item in lista)
            {
                item._Cuenta!.Movimientos = null;           // cuenta -> movimientos
                item._Cuenta._Cliente!.Cuentas = null;      // cliente -> cuentas
            }
        }
    }
}
