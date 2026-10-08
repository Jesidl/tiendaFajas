using libreria_parqueadero.entidades;

namespace libreria_parqueadero.interfaces
{
    public interface ICeldasAplicacion
    {
        Celdas Insertar(Celdas entidad);
        List<Celdas> Consultar();
        Celdas Actualizar(Celdas entidad);
        Celdas Borrar(Celdas entidad);

        List<Celdas> ConsultarDisponibles(string tipo);
    }
}
