using libreria_parqueadero.entidades;

namespace libreria_parqueadero.interfaces
{
    public interface IRegistrosAplicacion
    {
        Registros Insertar(Registros entidad);      // = ENTRADA
        List<Registros> Consultar();
        Registros Actualizar(Registros entidad);    // = MOVER de celda
        Registros Borrar(Registros entidad);

        Registros Salida(int id);
        List<Registros> ConsultarAbiertos();
        RecaudoDia Recaudo(DateTime fecha);
    }
}
