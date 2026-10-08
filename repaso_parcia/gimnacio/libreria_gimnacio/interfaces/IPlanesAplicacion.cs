using libreria_gimnacio.entidades;

namespace libreria_gimnacio.interfaces
{
    public interface IPlanesAplicacion  
    {
        Planes Insertar(Planes entidad);
        List<Planes> Consultar();
        Planes Actualizar(Planes entidad);
        Planes Borrar(Planes entidad);
    }
}