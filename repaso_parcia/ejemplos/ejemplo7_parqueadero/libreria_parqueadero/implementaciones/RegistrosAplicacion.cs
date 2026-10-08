using libreria_parqueadero.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_parqueadero.interfaces
{
    public class RegistrosAplicacion : IRegistrosAplicacion
    {
        // Tarifas
        private const int MINUTOS_GRACIA = 15;
        private const decimal HORA_CARRO = 4000m;
        private const decimal HORA_MOTO = 2000m;
        private const decimal TOPE_DIA_CARRO = 30000m;
        private const decimal TOPE_DIA_MOTO = 15000m;
        private const int MINUTOS_DIA = 1440;      // 24 * 60

        private IConexion conexion;

        public RegistrosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        // =====================================================================
        // INSERTAR = ENTRADA del vehiculo
        // =====================================================================
        public Registros Insertar(Registros entidad)
        {
            // ---- Valores iniciales ----
            if (entidad.Entrada == default(DateTime))
                entidad.Entrada = DateTime.Now;
            entidad.Salida = null;
            entidad.Minutos = 0;
            entidad.Valor = 0;
            entidad.Cerrado = false;

            // ---- Vehiculo ----
            Vehiculos? vehiculo;
            if (entidad._Vehiculo != null)
            {
                vehiculo = entidad._Vehiculo;
                vehiculo.Placa = (vehiculo.Placa ?? "").Replace(" ", "").ToUpper();
                this.conexion.Vehiculos!.Add(vehiculo);
            }
            else
            {
                vehiculo = this.conexion.Vehiculos!.FirstOrDefault(x => x.Id == entidad.Vehiculo);
                if (vehiculo == null)
                    throw new Exception("El vehiculo no esta registrado");

                // PATRON: NO PERMITIR DOS REGISTROS "ABIERTOS" A LA VEZ
                var yaAdentro = this.conexion.Registros!
                    .Any(x => x.Vehiculo == vehiculo.Id && !x.Cerrado);
                if (yaAdentro)
                    throw new Exception("El vehiculo " + vehiculo.Placa + " ya esta adentro");
            }

            // ---- Celda ----
            if (entidad._Celda != null)
            {
                // celda NUEVA (caso Parcial)
                entidad._Celda.Ocupada = true;
                this.conexion.Celdas!.Add(entidad._Celda);
            }
            else
            {
                Celdas? celda;

                if (entidad.Celda == 0)
                {
                    // PATRON: ASIGNACION AUTOMATICA -> la primera libre del tipo correcto
                    celda = this.conexion.Celdas!
                        .Where(x => !x.Ocupada && x.Tipo == vehiculo.Tipo)
                        .OrderBy(x => x.Codigo)
                        .FirstOrDefault();
                    if (celda == null)
                        throw new Exception("No hay celdas disponibles para " + vehiculo.Tipo);

                    entidad.Celda = celda.Id;    // se completa la FK con la celda encontrada
                }
                else
                {
                    celda = this.conexion.Celdas!.FirstOrDefault(x => x.Id == entidad.Celda);
                    if (celda == null)
                        throw new Exception("La celda no existe");
                    if (celda.Ocupada)
                        throw new Exception("La celda " + celda.Codigo + " esta ocupada");
                    // PATRON: COMPATIBILIDAD ENTRE 2 TABLAS
                    if (celda.Tipo != vehiculo.Tipo)
                        throw new Exception("La celda " + celda.Codigo + " es para " + celda.Tipo);
                }

                // Marcar la celda como ocupada (otra tabla)
                celda.Ocupada = true;
                var entryCelda = this.conexion.Entry<Celdas>(celda);
                entryCelda.State = EntityState.Modified;
            }

            this.conexion.Registros!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // =====================================================================
        // SALIDA: calcula tiempo y valor, libera la celda
        // =====================================================================
        public Registros Salida(int id)
        {
            var registro = this.conexion.Registros!.FirstOrDefault(x => x.Id == id);
            if (registro == null)
                throw new Exception("El registro no existe");
            if (registro.Cerrado)
                throw new Exception("Este registro ya tiene salida");

            var vehiculo = this.conexion.Vehiculos!.FirstOrDefault(x => x.Id == registro.Vehiculo);

            registro.Salida = DateTime.Now;

            // PATRON: TIEMPO TRANSCURRIDO
            // TotalMinutes = minutos con decimales (130.4). Math.Ceiling redondea hacia arriba (131).
            TimeSpan tiempo = registro.Salida.Value - registro.Entrada;
            registro.Minutos = (int)Math.Ceiling(tiempo.TotalMinutes);

            registro.Valor = CalcularValor(vehiculo!, registro.Minutos);
            registro.Cerrado = true;

            // Liberar la celda
            var celda = this.conexion.Celdas!.FirstOrDefault(x => x.Id == registro.Celda);
            if (celda != null)
            {
                celda.Ocupada = false;
                var entryCelda = this.conexion.Entry<Celdas>(celda);
                entryCelda.State = EntityState.Modified;
            }

            var entry = this.conexion.Entry<Registros>(registro);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();
            return registro;
        }

        // =====================================================================
        // ACTUALIZAR = MOVER el vehiculo a otra celda
        // PATRON: LIBERAR UNO Y OCUPAR OTRO (3 objetos modificados en 1 SaveChanges)
        // =====================================================================
        public Registros Actualizar(Registros entidad)
        {
            var original = this.conexion.Registros!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("El registro no existe");
            if (original.Cerrado)
                throw new Exception("El vehiculo ya salio");
            if (original.Celda == entidad.Celda)
                return original;                        // nada que cambiar

            var vehiculo = this.conexion.Vehiculos!.FirstOrDefault(x => x.Id == original.Vehiculo);
            var celdaNueva = this.conexion.Celdas!.FirstOrDefault(x => x.Id == entidad.Celda);
            if (celdaNueva == null)
                throw new Exception("La celda destino no existe");
            if (celdaNueva.Ocupada)
                throw new Exception("La celda destino esta ocupada");
            if (celdaNueva.Tipo != vehiculo!.Tipo)
                throw new Exception("La celda destino es para " + celdaNueva.Tipo);

            var celdaVieja = this.conexion.Celdas!.FirstOrDefault(x => x.Id == original.Celda);
            if (celdaVieja != null)
            {
                celdaVieja.Ocupada = false;
                this.conexion.Entry<Celdas>(celdaVieja).State = EntityState.Modified;
            }

            celdaNueva.Ocupada = true;
            this.conexion.Entry<Celdas>(celdaNueva).State = EntityState.Modified;

            original.Celda = celdaNueva.Id;
            this.conexion.Entry<Registros>(original).State = EntityState.Modified;

            this.conexion.SaveChanges();
            return original;
        }

        public Registros Borrar(Registros entidad)
        {
            var registro = this.conexion.Registros!.FirstOrDefault(x => x.Id == entidad.Id);
            if (registro == null)
                throw new Exception("El registro no existe");

            if (!registro.Cerrado)
            {
                var celda = this.conexion.Celdas!.FirstOrDefault(x => x.Id == registro.Celda);
                if (celda != null)
                {
                    celda.Ocupada = false;
                    this.conexion.Entry<Celdas>(celda).State = EntityState.Modified;
                }
            }

            this.conexion.Registros!.Remove(registro);
            this.conexion.SaveChanges();
            return registro;
        }

        public List<Registros> Consultar()
        {
            var lista = this.conexion.Registros!
                .Include(x => x._Vehiculo)
                .Include(x => x._Celda)
                .OrderByDescending(x => x.Entrada)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        // Vehiculos que estan adentro AHORA
        public List<Registros> ConsultarAbiertos()
        {
            var lista = this.conexion.Registros!
                .Include(x => x._Vehiculo)
                .Include(x => x._Celda)
                .Where(x => !x.Cerrado)
                .OrderBy(x => x.Entrada)
                .ToList();
            LimpiarCiclos(lista);
            return lista;
        }

        // =====================================================================
        // REPORTE: recaudo de un dia, agrupado por tipo de vehiculo
        // =====================================================================
        public RecaudoDia Recaudo(DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            var cerrados = this.conexion.Registros!
                .Include(x => x._Vehiculo)
                .Where(x => x.Cerrado && x.Salida >= inicio && x.Salida < fin)
                .ToList();

            var resultado = new RecaudoDia();
            resultado.Fecha = inicio;
            resultado.TotalVehiculos = cerrados.Count;
            resultado.TotalGeneral = cerrados.Sum(x => x.Valor);
            resultado.PorTipo = new List<RecaudoTipo>();

            // Agrupar por un campo de la tabla relacionada
            foreach (var grupo in cerrados.GroupBy(x => x._Vehiculo!.Tipo))
            {
                var item = new RecaudoTipo();
                item.Tipo = grupo.Key;
                item.Vehiculos = grupo.Count();
                item.MinutosTotales = grupo.Sum(x => x.Minutos);
                item.Total = grupo.Sum(x => x.Valor);
                resultado.PorTipo.Add(item);
            }
            return resultado;
        }

        // =====================================================================
        // CALCULO DE TARIFA (el mas "matematico")
        //   - mensualidad -> 0
        //   - primeros 15 min -> gratis
        //   - por hora o fraccion (61 min = 2 horas)
        //   - cada dia completo cobra el tope; el resto del tiempo no supera el tope
        // =====================================================================
        private decimal CalcularValor(Vehiculos vehiculo, int minutos)
        {
            if (vehiculo.Mensualidad)
                return 0;
            if (minutos <= MINUTOS_GRACIA)
                return 0;

            decimal tarifaHora = vehiculo.Tipo == "Moto" ? HORA_MOTO : HORA_CARRO;
            decimal topeDia = vehiculo.Tipo == "Moto" ? TOPE_DIA_MOTO : TOPE_DIA_CARRO;

            // PATRON: DIVISION ENTERA (/) Y RESIDUO (%)
            // 1580 min -> 1580 / 1440 = 1 dia completo ; 1580 % 1440 = 140 min sobrantes
            int dias = minutos / MINUTOS_DIA;
            int resto = minutos % MINUTOS_DIA;

            // "hora o fraccion": 140 min -> 140 / 60.0 = 2.33 -> Ceiling = 3 horas
            int horasResto = (int)Math.Ceiling(resto / 60.0);
            decimal valorResto = Math.Min(horasResto * tarifaHora, topeDia);   // Min = no pasar del tope

            return dias * topeDia + valorResto;
        }

        private void LimpiarCiclos(List<Registros> lista)
        {
            foreach (var item in lista)
            {
                item._Vehiculo!.Registros = null;
                item._Celda!.Registros = null;
            }
        }
    }
}
