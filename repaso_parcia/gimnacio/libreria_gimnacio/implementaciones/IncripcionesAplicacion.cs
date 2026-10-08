using libreria_gimnacio.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_gimnacio.interfaces
{
    public class InscripcionesAplicacion : IInscripcionesAplicacion
    {
        private IConexion conexion;

        public InscripcionesAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Inscripciones Actualizar(Inscripciones entidad)
        {
            // Si cambian la cantidad o el producto, el total debe recalcularse
           

            var entry = this.conexion!.Entry<Inscripciones>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Inscripciones Borrar(Inscripciones entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Inscripciones!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Inscripciones> Consultar()
        {
            var lista = this.conexion.Inscripciones!
                .Include(x => x._Plan)
                .Include(x => x._Socio)
                .ToList();

            foreach (var item in lista)
            {
                item._Socio!.Inscripciones = null;
                item._Plan!.Inscripciones = null;
            }
            return lista;
        }

        public Inscripciones Insertar(Inscripciones entidad)
        {

            // 1. Calcular el total (valida y busca el precio)
           

            // 2. Si vienen cliente o producto NUEVOS, se guardan primero
            if (entidad._Socio != null)
                this.conexion.Socios!.Add(entidad._Socio);

            if (entidad._Plan != null)
                this.conexion.Planes!.Add(entidad._Plan);
            
            // 3. Guardar la venta
            this.conexion.Inscripciones!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }


    

        }
    
}