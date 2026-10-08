using libreria_parqueadero.entidades;

namespace libreria_parqueadero.interfaces
{
    public interface IVehiculosAplicacion
    {
        Vehiculos Insertar(Vehiculos entidad);
        List<Vehiculos> Consultar();
        Vehiculos Actualizar(Vehiculos entidad);
        Vehiculos Borrar(Vehiculos entidad);
    }
}
