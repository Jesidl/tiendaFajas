using libreria_hotel.entidades;

namespace libreria_hotel.interfaces
{
    public interface IReservasAplicacion
    {
        Reservas Insertar(Reservas entidad);
        List<Reservas> Consultar();
        Reservas Actualizar(Reservas entidad);
        Reservas Borrar(Reservas entidad);
    }
}