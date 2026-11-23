using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IPedidosPresentacion
    {
        Pedidos Insertar(Pedidos entidad);
        Pedidos Actualizar(Pedidos entidad);
        List<Pedidos> Consultar();
        List<Pedidos> Buscar(Pedidos entidad);
        Pedidos Borrar(Pedidos entidad);
    }
}