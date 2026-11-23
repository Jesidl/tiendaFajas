using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Prueba del repositorio de Inventarios: recorre el ciclo completo contra la base
    /// de datos real y verifica cada paso leyendo de vuelta la fila, no solo
    /// que SaveChanges no haya lanzado.
    /// </summary>
    [TestClass]
    public class InventariosPrueba
    {
        private readonly IConexion conexion;
        private readonly EntidadesNucleo nucleo;
        private List<Inventarios>? lista;
        private Inventarios? entidad;

        public InventariosPrueba()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion, "Inventarios");
        }

        [TestMethod]
        public void Ejecutar()
        {
            this.nucleo.Iniciar();

            try
            {
                Assert.IsTrue(Guardar(), "no inserto Inventarios");
                Assert.IsTrue(Listar(), "no listo Inventarios");
                Assert.IsTrue(Buscar(), "no encontro el Inventarios insertado");
                Assert.IsTrue(Modificar(), "no actualizo Inventarios");
                Assert.IsTrue(Auditado(), "no quedo rastro de auditoria de Inventarios");
                Assert.IsTrue(Borrar(), "no borro Inventarios");
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
            this.entidad = this.nucleo.Inventarios();

            this.conexion.Inventarios!.Add(this.entidad);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Inventarios!, this.entidad);

            // IDENTITY asigna el Id al guardar: si sigue en cero no se inserto
            return this.entidad.Id > 0;
        }

        public bool Listar()
        {
            this.lista = this.conexion.Inventarios!.ToList();
            return this.lista.Count > 0;
        }

        public bool Buscar()
        {
            var llave = this.entidad!.Id;
            return this.conexion.Inventarios!.FirstOrDefault(x => x.Id == llave) != null;
        }

        public bool Modificar()
        {
            var llave = this.entidad!.Id;
            this.entidad!.Cantidad = 99;

            // entidad desconectada: se marca Modified a mano porque el
            // contexto corre con QueryTrackingBehavior.NoTracking
            var entry = this.conexion.Entry<Inventarios>(this.entidad);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();

            var leida = this.conexion.Inventarios!.FirstOrDefault(x => x.Id == llave);
            return leida != null && leida.Cantidad == 99;
        }

        /// <summary>
        /// La auditoria no se escribe en InventariosAplicacion sino interceptando
        /// SaveChanges en Conexion, asi que cada prueba de repositorio tambien
        /// comprueba que el rastro quedo. La etiqueta viaja por ContextoUsuario.
        /// </summary>
        public bool Auditado()
        {
            var llave = this.entidad!.Id;
            var etiqueta = this.nucleo.Etiqueta;

            var inserto = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Inventarios" &&
                x.Llave == llave &&
                x.Operacion == "Insertar" &&
                x.Usuario == etiqueta);

            var actualizo = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Inventarios" &&
                x.Llave == llave &&
                x.Operacion == "Actualizar" &&
                x.Usuario == etiqueta);

            return inserto && actualizo;
        }

        public bool Borrar()
        {
            var llave = this.entidad!.Id;

            this.conexion.Inventarios!.Remove(this.entidad!);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            return this.conexion.Inventarios!.FirstOrDefault(x => x.Id == llave) == null;
        }
    }
}