using libreria_hotel.entidades;

namespace libreria_hotel.interfaces
{
    public interface IHuespedesAplicacion
    {
        Huespedes Insertar(Huespedes entidad);
        List<Huespedes> Consultar();
        Huespedes Actualizar(Huespedes entidad);
        Huespedes Borrar(Huespedes entidad);
    }
}