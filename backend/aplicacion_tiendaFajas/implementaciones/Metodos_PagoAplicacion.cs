using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class Metodos_PagoAplicacion : IMetodos_PagoAplicacion
    {
        private IConexion IConexion;

        public Metodos_PagoAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Metodos_Pago Actualizar(Metodos_Pago entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Metodos_Pago>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Metodos_Pago Borrar(Metodos_Pago entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Metodos_Pago!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Metodos_Pago> Buscar(Metodos_Pago entidad)
        {
            return this.IConexion.Metodos_Pago!
                .Where(x => x.Nombre!.Contains(entidad.Nombre!))
                .ToList();
        }

        public List<Metodos_Pago> Consultar()
        {
            return this.IConexion.Metodos_Pago!
                .Take(5).ToList();
        }

        public Metodos_Pago Insertar(Metodos_Pago entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Metodos_Pago!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Metodos_Pago entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}