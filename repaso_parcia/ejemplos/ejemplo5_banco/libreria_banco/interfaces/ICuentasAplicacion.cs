using libreria_banco.entidades;

namespace libreria_banco.interfaces
{
    public interface ICuentasAplicacion
    {
        Cuentas Insertar(Cuentas entidad);
        List<Cuentas> Consultar();
        Cuentas Actualizar(Cuentas entidad);
        Cuentas Borrar(Cuentas entidad);

        List<Cuentas> ConsultarPorCliente(int cliente);
    }
}
