using libreria_tienda.entidades;

namespace libreria_tienda.interfaces
{
    public interface IVentasAplicacion
    {
        Ventas Insertar(Ventas entidad);
        List<Ventas> Consultar();
        Ventas Actualizar(Ventas entidad);
        Ventas Borrar(Ventas entidad);
    }
}