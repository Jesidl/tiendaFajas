using libreria_tienda.entidades;
using Microsoft.EntityFrameworkCore;

namespace libreria_tienda.interfaces
{
    public class VentasAplicacion : IVentasAplicacion
    {
        private IConexion conexion;

        public VentasAplicacion(IConexion conexion)
        {
            this.conexion = conexion;
        }

        public Ventas Actualizar(Ventas entidad)
        {
            // Si cambian la cantidad o el producto, el total debe recalcularse
            CalcularTotal(entidad);

            var entry = this.conexion!.Entry<Ventas>(entidad);
            entry.State = EntityState.Modified;
            this.conexion!.SaveChanges();
            return entidad;
        }

        public Ventas Borrar(Ventas entidad)
        {
            // Hacer calculo o operaciones

            this.conexion.Ventas!.Remove(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }

        public List<Ventas> Consultar()
        {
            var lista = this.conexion.Ventas!
                .Include(x => x._Producto)
                .Include(x => x._Cliente)
                .ToList();

            foreach (var item in lista)
            {
                item._Cliente!.Ventas = null;
                item._Producto!.Ventas = null;
            }
            return lista;
        }

        public Ventas Insertar(Ventas entidad)
        {

            // 1. Calcular el total (valida y busca el precio)
            CalcularTotal(entidad);

            // 2. Si vienen cliente o producto NUEVOS, se guardan primero
            if (entidad._Cliente != null)
                this.conexion.Clientes!.Add(entidad._Cliente);

            if (entidad._Producto != null)
                this.conexion.Productos!.Add(entidad._Producto);
            
            // 3. Guardar la venta
            this.conexion.Ventas!.Add(entidad!);
            this.conexion.SaveChanges();
            return entidad;
        }


        private void CalcularTotal(Ventas entidad)
        {
           if(entidad == null)
           
                throw new Exception("La Venta no puede ser nula");
           if  (entidad.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a cero");

            decimal precio;
            if (entidad._Producto != null)
            {
                // El producto viene completo en la venta (caso Parcial)
                precio = entidad._Producto.Precio;
            }
            else
            {
                // Solo viene el Id del producto: hay que buscarlo en la base
                var producto = this.conexion.Productos!
                    .FirstOrDefault(x => x.Id == entidad.Producto);
                if (producto == null)
                    throw new Exception("El producto no existe");
                precio = producto.Precio;
            }

            entidad.Total = entidad.Cantidad * precio;

        }
    }
}