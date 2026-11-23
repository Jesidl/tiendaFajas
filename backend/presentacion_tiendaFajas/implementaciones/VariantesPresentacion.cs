using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class VariantesPresentacion : IVariantesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public VariantesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Variantes Actualizar(Variantes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Variantes>(
                "Variantes/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Variantes Borrar(Variantes entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Variantes>(
                "Variantes/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Variantes> Buscar(Variantes entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Variantes>>(
                "Variantes/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Variantes> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Variantes>>(
                "Variantes/Consultar",
                new Variantes());
            task.Wait();
            return task.Result;
        }

        public Variantes Insertar(Variantes entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Variantes>(
                "Variantes/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Variantes entidad)
        {
            return string.IsNullOrEmpty(entidad.Sku) ||
                entidad.Producto == 0 ||
                entidad.Talla == 0 ||
                entidad.Color == 0;
        }
    }
}