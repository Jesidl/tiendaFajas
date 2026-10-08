using libreria_hotel.entidades;

namespace libreria_hotel.interfaces
{
    public interface IHabitacionesAplicacion
    {
        Habitaciones Insertar(Habitaciones entidad);
        List<Habitaciones> Consultar();
        Habitaciones Actualizar(Habitaciones entidad);
        Habitaciones Borrar(Habitaciones entidad);
    }
}