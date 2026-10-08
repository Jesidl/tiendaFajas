using libreria_tienda.entidades;

namespace libreria_tienda.interfaces
{
    public interface IProductosAplicacion
    {
        Productos Insertar(Productos entidad);
        List<Productos> Consultar();
        Productos Actualizar(Productos entidad);
        Productos Borrar(Productos entidad);
    }
}