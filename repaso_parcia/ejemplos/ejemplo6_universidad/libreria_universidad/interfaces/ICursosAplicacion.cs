using libreria_universidad.entidades;

namespace libreria_universidad.interfaces
{
    public interface ICursosAplicacion
    {
        Cursos Insertar(Cursos entidad);
        List<Cursos> Consultar();
        Cursos Actualizar(Cursos entidad);
        Cursos Borrar(Cursos entidad);
    }
}
