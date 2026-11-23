using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IMedidasAplicacion
    {
        Medidas Insertar(Medidas entidad);
        Medidas Actualizar(Medidas entidad);
        List<Medidas> Consultar();
        List<Medidas> Buscar(Medidas entidad);
        Medidas Borrar(Medidas entidad);
    }
}