using aplicacion_tiendaFajas.entidades;
using Microsoft.EntityFrameworkCore;

namespace aplicacion_tiendaFajas.interfaces
{
    public class ColoresAplicacion : IColoresAplicacion
    {
        private IConexion IConexion;

        public ColoresAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Colores Actualizar(Colores entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Colores>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Colores Borrar(Colores entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Colores!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Colores> Buscar(Colores entidad)
        {
            return this.IConexion.Colores!
                .Where(x => x.Nombre!.Contains(entidad.Nombre!))
                .ToList();
        }

        public List<Colores> Consultar()
        {
            return this.IConexion.Colores!
                .Take(5).ToList();
        }

        public Colores Insertar(Colores entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Colores!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Colores entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Codigo_Hex);
        }
    }
}