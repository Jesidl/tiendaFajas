using libreria_tienda.entidades;

namespace libreria_tienda.interfaces
{
    public interface IClientesAplicacion
    {
        Clientes Insertar(Clientes entidad);
        List<Clientes> Consultar();
        Clientes Actualizar(Clientes entidad);
        Clientes Borrar(Clientes entidad);
    }
}