using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Prueba del repositorio de Pedidos: recorre el ciclo completo contra la base
    /// de datos real y verifica cada paso leyendo de vuelta la fila, no solo
    /// que SaveChanges no haya lanzado.
    /// </summary>
    [TestClass]
    public class PedidosPrueba
    {
        private readonly IConexion conexion;
        private readonly EntidadesNucleo nucleo;
        private List<Pedidos>? lista;
        private Pedidos? entidad;

        public PedidosPrueba()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion, "Pedidos");
        }

        [TestMethod]
        public void Ejecutar()
        {
            this.nucleo.Iniciar();

            try
            {
                Assert.IsTrue(Guardar(), "no inserto Pedidos");
                Assert.IsTrue(Listar(), "no listo Pedidos");
                Assert.IsTrue(Buscar(), "no encontro el Pedidos insertado");
                Assert.IsTrue(Modificar(), "no actualizo Pedidos");
                Assert.IsTrue(Auditado(), "no quedo rastro de auditoria de Pedidos");
                Assert.IsTrue(Borrar(), "no borro Pedidos");
            }
            finally
            {
                // se deshace siempre, incluso si un Assert fallo, para que la
                // base no quede con filas de prueba
                this.nucleo.Limpiar();
            }
        }

        public bool Guardar()
        {
            this.entidad = this.nucleo.Pedidos();

            this.conexion.Pedidos!.Add(this.entidad);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Pedidos!, this.entidad);

            // IDENTITY asigna el Id al guardar: si sigue en cero no se inserto
            return this.entidad.Id > 0;
        }

        public bool Listar()
        {
            this.lista = this.conexion.Pedidos!.ToList();
            return this.lista.Count > 0;
        }

        public bool Buscar()
        {
            var llave = this.entidad!.Id;
            return this.conexion.Pedidos!.FirstOrDefault(x => x.Id == llave) != null;
        }

        public bool Modificar()
        {
            var llave = this.entidad!.Id;
            this.entidad!.Estado = "Recibido";

            // entidad desconectada: se marca Modified a mano porque el
            // contexto corre con QueryTrackingBehavior.NoTracking
            var entry = this.conexion.Entry<Pedidos>(this.entidad);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();

            var leida = this.conexion.Pedidos!.FirstOrDefault(x => x.Id == llave);
            return leida != null && leida.Estado == "Recibido";
        }

        /// <summary>
        /// La auditoria no se escribe en PedidosAplicacion sino interceptando
        /// SaveChanges en Conexion, asi que cada prueba de repositorio tambien
        /// comprueba que el rastro quedo. La etiqueta viaja por ContextoUsuario.
        /// </summary>
        public bool Auditado()
        {
            var llave = this.entidad!.Id;
            var etiqueta = this.nucleo.Etiqueta;

            var inserto = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Pedidos" &&
                x.Llave == llave &&
                x.Operacion == "Insertar" &&
                x.Usuario == etiqueta);

            var actualizo = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Pedidos" &&
                x.Llave == llave &&
                x.Operacion == "Actualizar" &&
                x.Usuario == etiqueta);

            return inserto && actualizo;
        }

        public bool Borrar()
        {
            var llave = this.entidad!.Id;

            this.conexion.Pedidos!.Remove(this.entidad!);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            return this.conexion.Pedidos!.FirstOrDefault(x => x.Id == llave) == null;
        }
    }
}