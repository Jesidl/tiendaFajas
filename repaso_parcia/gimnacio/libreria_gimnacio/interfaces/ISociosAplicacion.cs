using libreria_gimnacio.entidades;

namespace libreria_gimnacio.interfaces
{
    public interface ISociosAplicacion
    {
        Socios Insertar(Socios entidad);
        List<Socios> Consultar();
        Socios Actualizar(Socios entidad);
        Socios Borrar(Socios entidad);
    }
}