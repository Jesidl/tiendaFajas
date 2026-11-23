using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface ITallasAplicacion
    {
        Tallas Insertar(Tallas entidad);
        Tallas Actualizar(Tallas entidad);
        List<Tallas> Consultar();
        List<Tallas> Buscar(Tallas entidad);
        Tallas Borrar(Tallas entidad);
    }
}