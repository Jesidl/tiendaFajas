using libreria_parqueadero.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_parqueadero.interfaces
{
    public class CeldasAplicacion : ICeldasAplicacion
    {
        private IConexion conexion;

        public CeldasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Celdas Actualizar(Celdas entidad)
        {
            var original = this.conexion.Celdas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (original == null)
                throw new Exception("La celda no existe");

            // Campo protegido: "Ocupada" solo cambia con entradas/salidas
            entidad.Ocupada = original.Ocupada;

            if (original.Ocupada && entidad.Tipo != original.Tipo)
                throw new Exception("No se puede cambiar el tipo de una celda ocupada");

            var entry = this.conexion!.Entry<Celdas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Celdas Borrar(Celdas entidad)
        {
            var celda = this.conexion.Celdas!.FirstOrDefault(x => x.Id == entidad.Id);
            if (celda == null)
                throw new Exception("La celda no existe");
            if (celda.Ocupada)
                throw new Exception("La celda esta ocupada");
            if (this.conexion.Registros!.Any(x => x.Celda == celda.Id))
                throw new Exception("La celda tiene historial, no se puede borrar");

            this.conexion.Celdas!.Remove(celda);
            this.conexion.SaveChanges();
            return celda;
        }

        public List<Celdas> Consultar()
        {
            return this.conexion.Celdas!
                .OrderBy(x => x.Codigo)
                .ToList();
        }

        // GET /Celdas/Disponibles/Carro
        public List<Celdas> ConsultarDisponibles(string tipo)
        {
            return this.conexion.Celdas!
                .Where(x => !x.Ocupada && x.Tipo == tipo)
                .OrderBy(x => x.Codigo)
                .ToList();
        }

        public Celdas Insertar(Celdas entidad)
        {
            if (entidad.Tipo != "Carro" && entidad.Tipo != "Moto")
                throw new Exception("El tipo debe ser Carro o Moto");

            entidad.Codigo = (entidad.Codigo ?? "").Trim().ToUpper();
            if (entidad.Codigo == "")
                throw new Exception("El codigo es obligatorio");
            if (this.conexion.Celdas!.Any(x => x.Codigo == entidad.Codigo))
                throw new Exception("Ya existe la celda " + entidad.Codigo);

            entidad.Ocupada = false;

            this.conexion.Celdas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }
    }
}
