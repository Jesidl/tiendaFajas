using libreria_universidad.entidades;

namespace libreria_universidad.interfaces
{
    public interface IMatriculasAplicacion
    {
        Matriculas Insertar(Matriculas entidad);
        List<Matriculas> Consultar();
        Matriculas Actualizar(Matriculas entidad);
        Matriculas Borrar(Matriculas entidad);

        List<Matriculas> ConsultarPorEstudiante(int estudiante);
        Matriculas Calificar(int id, decimal nota);
        List<ResumenCurso> ResumenPorCurso(string periodo);
        PromedioEstudiante Promedio(int estudiante);
    }
}
