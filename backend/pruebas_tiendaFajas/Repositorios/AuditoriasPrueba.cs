using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Prueba del repositorio de Auditorias: recorre el ciclo completo contra la base
    /// de datos real y verifica cada paso leyendo de vuelta la fila, no solo
    /// que SaveChanges no haya lanzado.
    /// </summary>
    [TestClass]
    public class AuditoriasPrueba
    {
        private readonly IConexion conexion;
        private readonly EntidadesNucleo nucleo;
        private List<Auditorias>? lista;
        private Auditorias? entidad;

        public AuditoriasPrueba()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion, "Auditorias");
        }

        [TestMethod]
        public void Ejecutar()
        {
            this.nucleo.Iniciar();

            try
            {
                Assert.IsTrue(Guardar(), "no inserto Auditorias");
                Assert.IsTrue(Listar(), "no listo Auditorias");
                Assert.IsTrue(Buscar(), "no encontro el Auditorias insertado");
                Assert.IsTrue(Modificar(), "no actualizo Auditorias");
                Assert.IsTrue(Auditado(), "no quedo rastro de auditoria de Auditorias");
                Assert.IsTrue(Borrar(), "no borro Auditorias");
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
            this.entidad = this.nucleo.Auditorias();

            this.conexion.Auditorias!.Add(this.entidad);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Auditorias!, this.entidad);

            // IDENTITY asigna el Id al guardar: si sigue en cero no se inserto
            return this.entidad.Id > 0;
        }

        public bool Listar()
        {
            this.lista = this.conexion.Auditorias!.ToList();
            return this.lista.Count > 0;
        }

        public bool Buscar()
        {
            var llave = this.entidad!.Id;
            return this.conexion.Auditorias!.FirstOrDefault(x => x.Id == llave) != null;
        }

        public bool Modificar()
        {
            var llave = this.entidad!.Id;
            this.entidad!.Datos = "modificado por la prueba";

            // entidad desconectada: se marca Modified a mano porque el
            // contexto corre con QueryTrackingBehavior.NoTracking
            var entry = this.conexion.Entry<Auditorias>(this.entidad);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();

            var leida = this.conexion.Auditorias!.FirstOrDefault(x => x.Id == llave);
            return leida != null && leida.Datos == "modificado por la prueba";
        }

        /// <summary>
        /// Caso inverso al de las otras veintitres tablas: escribir en la
        /// bitacora no debe generar un rastro de la bitacora. Si lo generara,
        /// cada insercion escribiria una fila que a su vez escribiria otra, y
        /// SaveChanges no terminaria nunca.
        /// </summary>
        public bool Auditado()
        {
            var etiqueta = this.nucleo.Etiqueta;

            return !this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Auditorias" &&
                x.Usuario == etiqueta);
        }

        public bool Borrar()
        {
            var llave = this.entidad!.Id;

            this.conexion.Auditorias!.Remove(this.entidad!);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            return this.conexion.Auditorias!.FirstOrDefault(x => x.Id == llave) == null;
        }
    }
}