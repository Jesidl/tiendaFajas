using libreria_gimnacio.entidades;

namespace libreria_gimnacio.interfaces
{
    public interface IInscripcionesAplicacion
    {
        Inscripciones Insertar(Inscripciones entidad);
        List<Inscripciones> Consultar();
        Inscripciones Actualizar(Inscripciones entidad);
        Inscripciones Borrar(Inscripciones entidad);
    }
}