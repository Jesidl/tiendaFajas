using libreria_biblioteca.entidades;

namespace libreria_biblioteca.interfaces
{
    public interface ILectoresAplicacion
    {
        Lectores Insertar(Lectores entidad);
        List<Lectores> Consultar();
        Lectores Actualizar(Lectores entidad);
        Lectores Borrar(Lectores entidad);
    }
}
