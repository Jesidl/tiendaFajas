using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IDetalles_PedidosAplicacion
    {
        Detalles_Pedidos Insertar(Detalles_Pedidos entidad);
        Detalles_Pedidos Actualizar(Detalles_Pedidos entidad);
        List<Detalles_Pedidos> Consultar();
        List<Detalles_Pedidos> Buscar(Detalles_Pedidos entidad);
        Detalles_Pedidos Borrar(Detalles_Pedidos entidad);
    }
}