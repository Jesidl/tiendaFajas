using libreria_clinica.entidades;

namespace libreria_clinica.interfaces
{
    public interface ICitasAplicacion
    {
        // Los 4 de siempre
        Citas Insertar(Citas entidad);
        List<Citas> Consultar();
        Citas Actualizar(Citas entidad);
        Citas Borrar(Citas entidad);

        // EXTRA: metodos que pide el enunciado.
        // REGLA DE ORO: todo metodo nuevo en CitasAplicacion debe estar aqui,
        // si no, el controller (que usa la interfaz) no lo ve.
        List<Citas> ConsultarPorMedico(int medico);
        Citas Atender(int id, string? observacion);
    }
}
