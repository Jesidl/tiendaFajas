using libreria_notas.entidades;

namespace libreria_notas.interfaces
{
    public interface IMateriasAplicacion
    {
        Materias Insertar(Materias entidad);
        List<Materias> Consultar();
        Materias Actualizar(Materias entidad);
        Materias Borrar(Materias entidad);
    }
}