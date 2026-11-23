using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IDetalles_VentasPresentacion
    {
        Detalles_Ventas Insertar(Detalles_Ventas entidad);
        Detalles_Ventas Actualizar(Detalles_Ventas entidad);
        List<Detalles_Ventas> Consultar();
        List<Detalles_Ventas> Buscar(Detalles_Ventas entidad);
        Detalles_Ventas Borrar(Detalles_Ventas entidad);
    }
}