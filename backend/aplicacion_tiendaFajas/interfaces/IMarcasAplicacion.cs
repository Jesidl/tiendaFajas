using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IMarcasAplicacion
    {
        Marcas Insertar(Marcas entidad);
        Marcas Actualizar(Marcas entidad);
        List<Marcas> Consultar();
        List<Marcas> Buscar(Marcas entidad);
        Marcas Borrar(Marcas entidad);
    }
}