using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IMarcasPresentacion
    {
        Marcas Insertar(Marcas entidad);
        Marcas Actualizar(Marcas entidad);
        List<Marcas> Consultar();
        List<Marcas> Buscar(Marcas entidad);
        Marcas Borrar(Marcas entidad);
    }
}