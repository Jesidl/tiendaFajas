using libreria_clinica.entidades;

namespace libreria_clinica.interfaces
{
    public interface IMedicosAplicacion
    {
        Medicos Insertar(Medicos entidad);
        List<Medicos> Consultar();
        Medicos Actualizar(Medicos entidad);
        Medicos Borrar(Medicos entidad);
    }
}
