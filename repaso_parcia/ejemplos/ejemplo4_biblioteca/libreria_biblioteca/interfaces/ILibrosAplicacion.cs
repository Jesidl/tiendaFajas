using libreria_biblioteca.entidades;

namespace libreria_biblioteca.interfaces
{
    public interface ILibrosAplicacion
    {
        Libros Insertar(Libros entidad);
        List<Libros> Consultar();
        Libros Actualizar(Libros entidad);
        Libros Borrar(Libros entidad);

        List<Libros> ConsultarDisponibles();
    }
}
