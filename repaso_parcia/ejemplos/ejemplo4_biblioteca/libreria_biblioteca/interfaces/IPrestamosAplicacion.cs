using libreria_biblioteca.entidades;

namespace libreria_biblioteca.interfaces
{
    public interface IPrestamosAplicacion
    {
        Prestamos Insertar(Prestamos entidad);
        List<Prestamos> Consultar();
        Prestamos Actualizar(Prestamos entidad);
        Prestamos Borrar(Prestamos entidad);

        // Acciones / consultas extra del enunciado
        Prestamos Devolver(int id);
        List<Prestamos> ConsultarPendientes();
        List<Prestamos> ConsultarVencidos();
        List<Prestamos> ConsultarPorLector(int lector);
    }
}
