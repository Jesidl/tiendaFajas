using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IPermisosPresentacion
    {
        Permisos Insertar(Permisos entidad);
        Permisos Actualizar(Permisos entidad);
        List<Permisos> Consultar();
        List<Permisos> Buscar(Permisos entidad);
        Permisos Borrar(Permisos entidad);
    }
}