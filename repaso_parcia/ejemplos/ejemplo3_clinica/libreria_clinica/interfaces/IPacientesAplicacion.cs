using libreria_clinica.entidades;

namespace libreria_clinica.interfaces
{
    public interface IPacientesAplicacion
    {
        Pacientes Insertar(Pacientes entidad);
        List<Pacientes> Consultar();
        Pacientes Actualizar(Pacientes entidad);
        Pacientes Borrar(Pacientes entidad);
    }
}
