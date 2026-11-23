using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface ISucursalesAplicacion
    {
        Sucursales Insertar(Sucursales entidad);
        Sucursales Actualizar(Sucursales entidad);
        List<Sucursales> Consultar();
        List<Sucursales> Buscar(Sucursales entidad);
        Sucursales Borrar(Sucursales entidad);
    }
}