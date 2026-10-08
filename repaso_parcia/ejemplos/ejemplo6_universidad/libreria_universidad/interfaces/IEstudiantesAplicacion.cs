using libreria_universidad.entidades;

namespace libreria_universidad.interfaces
{
    public interface IEstudiantesAplicacion
    {
        Estudiantes Insertar(Estudiantes entidad);
        List<Estudiantes> Consultar();
        Estudiantes Actualizar(Estudiantes entidad);
        Estudiantes Borrar(Estudiantes entidad);
    }
}
