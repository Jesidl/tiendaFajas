using libreria_banco.entidades;

namespace libreria_banco.interfaces
{
    public interface IMovimientosAplicacion
    {
        Movimientos Insertar(Movimientos entidad);
        List<Movimientos> Consultar();
        Movimientos Actualizar(Movimientos entidad);
        Movimientos Borrar(Movimientos entidad);

        List<Movimientos> ConsultarPorCuenta(int cuenta);
        Extracto Extracto(int cuenta);     // devuelve el DTO, no una entidad
    }
}
