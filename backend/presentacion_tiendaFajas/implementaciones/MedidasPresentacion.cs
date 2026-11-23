using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.interfaces;

namespace aplicacion_tiendaFajas.implementaciones
{
    public class MedidasPresentacion : IMedidasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public MedidasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Medidas Actualizar(Medidas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Medidas>(
                "Medidas/Actualizar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public Medidas Borrar(Medidas entidad)
        {
            if (entidad.Id == 0)
                throw new Exception("No se ha guardo");

            var task = this.IComunicaciones.Ejecutar<Medidas>(
                "Medidas/Borrar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Medidas> Buscar(Medidas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Medidas>>(
                "Medidas/Buscar",
                entidad);
            task.Wait();
            return task.Result;
        }

        public List<Medidas> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Medidas>>(
                "Medidas/Consultar",
                new Medidas());
            task.Wait();
            return task.Result;
        }

        public Medidas Insertar(Medidas entidad)
        {
            if (entidad.Id != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var task = this.IComunicaciones.Ejecutar<Medidas>(
                "Medidas/Insertar",
                entidad);
            task.Wait();
            return task.Result;
        }

        private bool Validar(Medidas entidad)
        {
            return entidad.Cliente == 0 ||
                entidad.Fecha == null;
        }
    }
}