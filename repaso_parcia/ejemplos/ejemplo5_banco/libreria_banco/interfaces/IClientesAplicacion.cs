using libreria_banco.entidades;

namespace libreria_banco.interfaces
{
    public interface IClientesAplicacion
    {
        Clientes Insertar(Clientes entidad);
        List<Clientes> Consultar();
        Clientes Actualizar(Clientes entidad);
        Clientes Borrar(Clientes entidad);
    }
}
