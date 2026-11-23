using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IMaterialesAplicacion
    {
        Materiales Insertar(Materiales entidad);
        Materiales Actualizar(Materiales entidad);
        List<Materiales> Consultar();
        List<Materiales> Buscar(Materiales entidad);
        Materiales Borrar(Materiales entidad);
    }
}