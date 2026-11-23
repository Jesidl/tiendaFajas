using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface ICategoriasAplicacion
    {
        Categorias Insertar(Categorias entidad);
        Categorias Actualizar(Categorias entidad);
        List<Categorias> Consultar();
        List<Categorias> Buscar(Categorias entidad);
        Categorias Borrar(Categorias entidad);
    }
}