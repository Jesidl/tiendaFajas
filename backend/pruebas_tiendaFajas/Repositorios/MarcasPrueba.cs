using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Prueba del repositorio de Marcas: recorre el ciclo completo contra la base
    /// de datos real y verifica cada paso leyendo de vuelta la fila, no solo
    /// que SaveChanges no haya lanzado.
    /// </summary>
    [TestClass]
    public class MarcasPrueba
    {
        private readonly IConexion conexion;
        private readonly EntidadesNucleo nucleo;
        private List<Marcas>? lista;
        private Marcas? entidad;

        public MarcasPrueba()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion, "Marcas");
        }

        [TestMethod]
        public void Ejecutar()
        {
            this.nucleo.Iniciar();

            try
            {
                Assert.IsTrue(Guardar(), "no inserto Marcas");
                Assert.IsTrue(Listar(), "no listo Marcas");
                Assert.IsTrue(Buscar(), "no encontro el Marcas insertado");
                Assert.IsTrue(Modificar(), "no actualizo Marcas");
                Assert.IsTrue(Auditado(), "no quedo rastro de auditoria de Marcas");
                Assert.IsTrue(Borrar(), "no borro Marcas");
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
            this.entidad = this.nucleo.Marcas();

            this.conexion.Marcas!.Add(this.entidad);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Marcas!, this.entidad);

            // IDENTITY asigna el Id al guardar: si sigue en cero no se inserto
            return this.entidad.Id > 0;
        }

        public bool Listar()
        {
            this.lista = this.conexion.Marcas!.ToList();
            return this.lista.Count > 0;
        }

        public bool Buscar()
        {
            var llave = this.entidad!.Id;
            return this.conexion.Marcas!.FirstOrDefault(x => x.Id == llave) != null;
        }

        public bool Modificar()
        {
            var llave = this.entidad!.Id;
            this.entidad!.Pais = "Mexico";

            // entidad desconectada: se marca Modified a mano porque el
            // contexto corre con QueryTrackingBehavior.NoTracking
            var entry = this.conexion.Entry<Marcas>(this.entidad);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();

            var leida = this.conexion.Marcas!.FirstOrDefault(x => x.Id == llave);
            return leida != null && leida.Pais == "Mexico";
        }

        /// <summary>
        /// La auditoria no se escribe en MarcasAplicacion sino interceptando
        /// SaveChanges en Conexion, asi que cada prueba de repositorio tambien
        /// comprueba que el rastro quedo. La etiqueta viaja por ContextoUsuario.
        /// </summary>
        public bool Auditado()
        {
            var llave = this.entidad!.Id;
            var etiqueta = this.nucleo.Etiqueta;

            var inserto = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Marcas" &&
                x.Llave == llave &&
                x.Operacion == "Insertar" &&
                x.Usuario == etiqueta);

            var actualizo = this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Marcas" &&
                x.Llave == llave &&
                x.Operacion == "Actualizar" &&
                x.Usuario == etiqueta);

            return inserto && actualizo;
        }

        public bool Borrar()
        {
            var llave = this.entidad!.Id;

            this.conexion.Marcas!.Remove(this.entidad!);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            return this.conexion.Marcas!.FirstOrDefault(x => x.Id == llave) == null;
        }
    }
}