using aplicacion_tiendaFajas.entidades;

namespace aplicacion_tiendaFajas.interfaces
{
    public interface IMetodos_PagoAplicacion
    {
        Metodos_Pago Insertar(Metodos_Pago entidad);
        Metodos_Pago Actualizar(Metodos_Pago entidad);
        List<Metodos_Pago> Consultar();
        List<Metodos_Pago> Buscar(Metodos_Pago entidad);
        Metodos_Pago Borrar(Metodos_Pago entidad);
    }
}