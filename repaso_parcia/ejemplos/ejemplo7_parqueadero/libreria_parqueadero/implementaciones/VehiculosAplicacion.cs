using libreria_parqueadero.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_parqueadero.interfaces
{
    public class VehiculosAplicacion : IVehiculosAplicacion
    {
        private IConexion conexion;

        public VehiculosAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Vehiculos Actualizar(Vehiculos entidad)
        {
            Normalizar(entidad);
            Validar(entidad);

            var repetida = this.conexion.Vehiculos!
                .Any(x => x.Placa == entidad.Placa && x.Id != entidad.Id);
            if (repetida)
                throw new Exception("Ya existe otro vehiculo con la placa " + entidad.Placa);

            var entry = this.conexion!.Entry<Vehiculos>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Vehiculos Borrar(Vehiculos entidad)
        {
            var adentro = this.conexion.Registros!.Any(x => x.Vehiculo == entidad.Id && !x.Cerrado);
            if (adentro)
                throw new Exception("El vehiculo esta dentro del parqueadero");

            this.conexion.Vehiculos!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Vehiculos> Consultar()
        {
            return this.conexion.Vehiculos!
                .OrderBy(x => x.Placa)
                .ToList();
        }

        public Vehiculos Insertar(Vehiculos entidad)
        {
            Normalizar(entidad);
            Validar(entidad);

            var repetida = this.conexion.Vehiculos!.Any(x => x.Placa == entidad.Placa);
            if (repetida)
                throw new Exception("Ya existe un vehiculo con la placa " + entidad.Placa);

            this.conexion.Vehiculos!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        // PATRON: NORMALIZAR TEXTO antes de validar y guardar
        // " abc 123 " -> "ABC123". Asi la busqueda por placa siempre coincide.
        private void Normalizar(Vehiculos entidad)
        {
            entidad.Placa = (entidad.Placa ?? "").Replace(" ", "").Replace("-", "").ToUpper();
        }

        private void Validar(Vehiculos entidad)
        {
            if (entidad.Placa!.Length != 6)
                throw new Exception("La placa debe tener 6 caracteres");
            if (entidad.Tipo != "Carro" && entidad.Tipo != "Moto")
                throw new Exception("El tipo debe ser Carro o Moto");
        }
    }
}
