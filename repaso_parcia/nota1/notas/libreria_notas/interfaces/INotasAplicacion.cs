using libreria_notas.entidades;

namespace libreria_notas.interfaces
{
    public interface INotasAplicacion
    {
        Notas Insertar(Notas entidad);
        List<Notas> Consultar();
        Notas Actualizar(Notas entidad);
        Notas Borrar(Notas entidad);
    }
}