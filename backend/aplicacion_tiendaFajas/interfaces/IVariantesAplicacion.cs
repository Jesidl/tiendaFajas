using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IVariantesAplicacion
    {
        Variantes Insertar(Variantes entidad);
        Variantes Actualizar(Variantes entidad);
        List<Variantes> Consultar();
        List<Variantes> Buscar(Variantes entidad);
        Variantes Borrar(Variantes entidad);
    }
}