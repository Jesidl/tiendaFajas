using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IRolesPresentacion
    {
        Roles Insertar(Roles entidad);
        Roles Actualizar(Roles entidad);
        List<Roles> Consultar();
        List<Roles> Buscar(Roles entidad);
        Roles Borrar(Roles entidad);
    }
}