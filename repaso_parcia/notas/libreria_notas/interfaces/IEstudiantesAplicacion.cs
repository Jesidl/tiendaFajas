using libreria_notas.entidades;

namespace libreria_notas.interfaces
{
    public interface IEstudiantesAplicacion
    {
        Estudiantes Insertar(Estudiantes entidad);
        List<Estudiantes> Consultar();
        Estudiantes Actualizar(Estudiantes entidad);
        Estudiantes Borrar(Estudiantes entidad);
    }
}