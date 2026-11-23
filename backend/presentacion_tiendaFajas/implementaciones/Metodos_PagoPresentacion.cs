using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class Metodos_PagoPresentacion : IMetodos_PagoPresentacion
    {
        private IComunicaciones IComunicaciones;

        public Metodos_PagoPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Metodos_Pago Actualizar(Metodos_Pago entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Metodos_Pago>(
                "Metodos_Pago/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Metodos_Pago Borrar(Metodos_Pago entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Metodos_Pago>(
                "Metodos_Pago/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Metodos_Pago> Buscar(Metodos_Pago entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Metodos_Pago>>(
                "Metodos_Pago/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Metodos_Pago> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Metodos_Pago>>(
                "Metodos_Pago/Consultar",
                new Metodos_Pago());
            task.Wait();
            return task.Result;
        }

        public Metodos_Pago Insertar(Metodos_Pago entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Metodos_Pago>(
                "Metodos_Pago/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Metodos_Pago entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}