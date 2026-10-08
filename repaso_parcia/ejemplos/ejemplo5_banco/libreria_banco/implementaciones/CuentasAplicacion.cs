using libreria_banco.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_banco.interfaces
{
    public class CuentasAplicacion : ICuentasAplicacion
    {
        private const decimal SALDO_MINIMO_AHORROS = 50000m;
        private const int MAX_CUENTAS_POR_CLIENTE = 3;

        private IConexion conexion;

        public CuentasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR = ABRIR CUENTA
        // =====================================================================
        public Cuentas Insertar(Cuentas entidad)
        {
            // PATRON: VALIDAR CONTRA UNA LISTA DE VALORES PERMITIDOS
            if (entidad.Tipo != "Ahorros" && entidad.Tipo != "Corriente")
                throw new Exception("El tipo debe ser Ahorros o Corriente");

            // PATRON: MINIMO SEGUN EL TIPO
            if (entidad.Tipo == "Ahorros" && entidad.Saldo < SALDO_MINIMO_AHORROS)
                throw new Exception("La cuenta de ahorros se abre con minimo " + SALDO_MINIMO_AHORROS);
            if (entidad.Saldo < 0)
                throw new Exception("El saldo inicial no puede ser negativo");

            // ---- Cliente: nuevo o existente ----
            if (entidad._Cliente != null)
            {
                entidad._Cliente.Activo = true;
                this.conexion.Clientes!.Add(entidad._Cliente);
            }
            else
            {
                var cliente = this.conexion.Clientes!.FirstOrDefault(x => x.Id == entidad.Cliente);
                if (cliente == null || !cliente.Activo)
                    throw new Exception("El cliente no existe o esta inactivo");

                int cuentasActivas = this.conexion.Cuentas!.Count(x => x.Cliente == entidad.Cliente && x.Activa);
                if (cuentasActivas >= MAX_CUENTAS_POR_CLIENTE)
                    throw new Exception("El cliente ya tiene el maximo de " + MAX_CUENTAS_POR_CLIENTE + " cuentas");
            }

            // ---- Valores generados por el sistema ----
            entidad.Numero = GenerarNumero(entidad.Tipo);
            entidad.FechaApertura = DateTime.Now;
            entidad.Activa = true;

            // PATRON: INSERTAR AUTOMATICAMENTE EN OTRA TABLA
            // Si la cuenta se abre con dinero, queda registrado como primer movimiento.
            // Se enlaza por la propiedad de navegacion _Cuenta (todavia no hay Id):
            // EF inserta primero la cuenta, obtiene su Id y lo copia en mov.Cuenta.
            if (entidad.Saldo > 0)
            {
                var mov = new Movimientos();
                mov._Cuenta = entidad;
                mov.Tipo = "Apertura";
                mov.Valor = entidad.Saldo;
                mov.Impuesto = 0;
                mov.SaldoAnterior = 0;
                mov.SaldoNuevo = entidad.Saldo;
                mov.Fecha = entidad.FechaApertura;
                mov.Descripcion = "Saldo inicial";
                this.conexion.Movimientos!.Add(mov);
            }

            this.conexion.Cuentas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // ACTUALIZAR
        // PATRON: PROTEGER CAMPOS -> el Saldo NUNCA se cambia por PUT
        // (solo con movimientos), ni el numero, ni el cliente, ni la fecha.
        // =====================================================================
        public Cuentas Actualizar(Cuentas entidad)
        {
            var original = this.conexion.Cuentas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("La cuenta no existe");

            // Se copian del ORIGINAL los campos que el usuario no puede tocar
            entidad.Saldo = original.Saldo;
            entidad.Numero = original.Numero;
            entidad.Cliente = original.Cliente;
            entidad.FechaApertura = original.FechaApertura;
            // Tipo no se puede cambiar si cambia el formato del numero
            entidad.Tipo = original.Tipo;
            // -> lo unico editable es "Activa" (bloquear / desbloquear)

            entidad._Cliente = null;
            entidad.Movimientos = null;

            var entry = this.conexion!.Entry<Cuentas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Cuentas Borrar(Cuentas entidad)
        {
            var cuenta = this.conexion.Cuentas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (cuenta == null)
                throw new Exception("La cuenta no existe");

            // PATRON: CONDICION SOBRE UN VALOR para permitir borrar
            if (cuenta.Saldo != 0)
                throw new Exception("La cuenta debe tener saldo 0 para cerrarse. Saldo actual: " + cuenta.Saldo);

            // Borrado fisico: por el Cascade de la FK se borran tambien sus movimientos
            this.conexion.Cuentas!.Remove(cuenta);
            this.conexion.SaveChanges();
            return cuenta;
        }

        public List<Cuentas> Consultar()
        {
            var lista = this.conexion.Cuentas!
                .Include(x => x._Cliente)
                .OrderBy(x => x.Numero)
                .ToList();
            foreach (var item in lista)
                item._Cliente!.Cuentas = null;
            return lista;
        }

        public List<Cuentas> ConsultarPorCliente(int cliente)
        {
            var lista = this.conexion.Cuentas!
                .Include(x => x._Cliente)
                .Where(x => x.Cliente == cliente)
                .ToList();
            foreach (var item in lista)
                item._Cliente!.Cuentas = null;
            return lista;
        }

        // PATRON: GENERAR UN CODIGO / CONSECUTIVO
        // AH-000001, CO-000002 ... :D6 = rellenar con ceros hasta 6 digitos
        private string GenerarNumero(string? tipo)
        {
            string prefijo = tipo == "Ahorros" ? "AH" : "CO";   // operador ternario: condicion ? si : no
            int consecutivo = this.conexion.Cuentas!.Count() + 1;
            string numero = prefijo + "-" + consecutivo.ToString("D6");

            // Por si se borraron cuentas y el numero ya existe
            while (this.conexion.Cuentas!.Any(x => x.Numero == numero))
            {
                consecutivo++;
                numero = prefijo + "-" + consecutivo.ToString("D6");
            }
            return numero;
        }
    }
}
