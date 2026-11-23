using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IColoresAplicacion
    {
        Colores Insertar(Colores entidad);
        Colores Actualizar(Colores entidad);
        List<Colores> Consultar();
        List<Colores> Buscar(Colores entidad);
        Colores Borrar(Colores entidad);
    }
}