using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Pruebas del modulo de auditoria.
    ///
    /// La auditoria no esta escrita en las veinticuatro clases XAplicacion
    /// sino interceptando SaveChanges en Conexion, para no repetir la misma
    /// logica setenta y dos veces. Esa decision concentra el riesgo: un error
    /// aqui afecta a todas las entidades a la vez, y como ninguna clase de
    /// negocio la menciona, nada en el codigo de arriba lo delataria.
    ///
    /// Cada prueba de repositorio comprueba que su propia tabla deja rastro.
    /// Esta clase prueba el mecanismo en si: el usuario que se registra, los
    /// datos que nunca se deben registrar, el comportamiento cuando la
    /// escritura falla, y el manejo del ChangeTracker.
    ///
    /// A diferencia de las pruebas de repositorio, aqui hay varios metodos de
    /// prueba en una sola clase, y corren en paralelo. Por eso la conexion y
    /// la etiqueta se arman en TestInitialize con el nombre del metodo: si los
    /// seis compartieran etiqueta, la limpieza de uno se llevaria los rastros
    /// que otro todavia esta comprobando.
    /// </summary>
    [TestClass]
    public class AuditoriaTransversalPrueba
    {
        public TestContext TestContext { get; set; } = null!;

        // se usa el tipo concreto y no IConexion porque una prueba necesita
        // mirar el ChangeTracker, que la interfaz no expone
        private Conexion conexion = null!;
        private EntidadesNucleo nucleo = null!;

        [TestInitialize]
        public void Preparar()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion,
                "Auditoria." + this.TestContext.TestName);
            this.nucleo.Iniciar();
        }

        [TestCleanup]
        public void Terminar()
        {
            // Se suelta todo lo que el contexto tenga pendiente antes de
            // limpiar. Una prueba puede dejar una entidad en estado Added que
            // no se pudo guardar -eso es justamente lo que comprueba
            // UnaEscrituraQueFallaNoDejaRastro-, y entonces el primer
            // SaveChanges de la limpieza volveria a intentar guardarla y
            // fallaria otra vez. Soltarlas no estorba a la limpieza: Remove
            // sobre una entidad desconectada la vuelve a enganchar por su Id.
            foreach (var entrada in this.conexion.ChangeTracker.Entries().ToList())
                entrada.State = EntityState.Detached;

            this.nucleo.Limpiar();
        }

        [TestMethod]
        public void LasTresOperacionesDejanRastroConSuUsuario()
        {
            var etiqueta = this.nucleo.Etiqueta;

            var cargo = this.nucleo.Cargos();
            this.conexion.Cargos!.Add(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Cargos!, cargo);

            var llave = cargo.Id;

            cargo.Descripcion = "cambiada";
            this.conexion.Entry(cargo).State = EntityState.Modified;
            this.conexion.SaveChanges();

            this.conexion.Cargos!.Remove(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            var rastros = this.conexion.Auditorias!
                .Where(x => x.Entidad == "Cargos"
                    && x.Llave == llave
                    && x.Usuario == etiqueta)
                .Select(x => x.Operacion)
                .ToList();

            Assert.Contains("Insertar", rastros, "falta el rastro de Insertar");
            Assert.Contains("Actualizar", rastros, "falta el rastro de Actualizar");
            Assert.Contains("Borrar", rastros, "falta el rastro de Borrar");
        }

        [TestMethod]
        public void SinUsuarioEnContextoLaAuditoriaDiceSistema()
        {
            // las tareas internas y cualquier escritura que no venga de una
            // sesion tienen que quedar atribuidas a algo, no a un campo vacio
            ContextoUsuario.Usuario = null;

            var cargo = this.nucleo.Cargos();
            this.conexion.Cargos!.Add(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Cargos!, cargo);

            var llave = cargo.Id;

            var usuario = this.conexion.Auditorias!
                .Where(x => x.Entidad == "Cargos" && x.Llave == llave)
                .Select(x => x.Usuario)
                .FirstOrDefault();

            Assert.AreEqual(ContextoUsuario.Anonimo, usuario,
                "una escritura sin sesion debe quedar atribuida a '" +
                ContextoUsuario.Anonimo + "'");

            // esta es la unica prueba cuyos rastros no llevan la etiqueta, asi
            // que se los lleva ella misma por llave. Se borra primero la fila,
            // porque borrarla tambien escribe un rastro.
            this.conexion.Cargos!.Remove(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            Borrar("Cargos", llave);
        }

        [TestMethod]
        public void LaAuditoriaNoRegistraLaClave()
        {
            var etiqueta = this.nucleo.Etiqueta;

            var usuario = this.nucleo.Usuarios();
            this.conexion.Usuarios!.Add(usuario);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Usuarios!, usuario);

            var llave = usuario.Id;

            var datos = this.conexion.Auditorias!
                .Where(x => x.Entidad == "Usuarios"
                    && x.Llave == llave
                    && x.Usuario == etiqueta)
                .Select(x => x.Datos)
                .FirstOrDefault();

            Assert.IsNotNull(datos, "no quedo rastro del alta de Usuarios");
            Assert.DoesNotContain("Clave", datos!,
                "la bitacora esta registrando el campo Clave");
            Assert.DoesNotContain(usuario.Clave!, datos!,
                "la bitacora esta registrando el hash de la contrasena: " +
                "un atacante con acceso de lectura a la bitacora tendria los " +
                "hashes sin necesidad de tocar la tabla Usuarios");
            Assert.Contains("Correo", datos!,
                "la bitacora deberia registrar los demas campos");
        }

        [TestMethod]
        public void LaAuditoriaNoRegistraElIdTemporalDeEntityFramework()
        {
            // el texto de Datos se arma antes de que SaveChanges llegue a la
            // base, cuando el Id todavia es el temporal negativo que asigna
            // Entity Framework. El Id definitivo va en la columna Llave.
            var etiqueta = this.nucleo.Etiqueta;

            var cargo = this.nucleo.Cargos();
            this.conexion.Cargos!.Add(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Cargos!, cargo);

            var llave = cargo.Id;

            var rastro = this.conexion.Auditorias!
                .Where(x => x.Entidad == "Cargos"
                    && x.Llave == llave
                    && x.Usuario == etiqueta)
                .FirstOrDefault();

            Assert.IsNotNull(rastro, "no quedo rastro");
            Assert.DoesNotContain("Id=", rastro!.Datos!,
                "la bitacora esta registrando el campo Id: en una insercion " +
                "todavia vale el temporal negativo de Entity Framework, que " +
                "no corresponde a ninguna fila");
            Assert.AreEqual(llave, rastro.Llave,
                "la columna Llave no tiene el Id definitivo");
        }

        [TestMethod]
        public void UnaEscrituraQueFallaNoDejaRastro()
        {
            // el rastro y la fila se escriben dentro de una misma transaccion,
            // asi que no puede quedar auditoria de algo que no se guardo
            var etiqueta = this.nucleo.Etiqueta;

            var empleado = new Empleados
            {
                Cedula = EntidadesNucleo.Unico("X", 50),
                Nombre = "empleado imposible",
                Telefono = "3000000000",
                Sucursal = 999999999,   // no existe: viola la llave foranea
                Cargo = 999999999
            };

            this.conexion.Empleados!.Add(empleado);

            var fallo = false;
            try
            {
                this.conexion.SaveChanges();
            }
            catch
            {
                fallo = true;
            }

            Assert.IsTrue(fallo,
                "la base acepto una llave foranea que no existe: revisar las " +
                "restricciones de la tabla Empleados");

            var rastros = this.conexion.Auditorias!
                .Count(x => x.Entidad == "Empleados" && x.Usuario == etiqueta);

            Assert.AreEqual(0, rastros,
                "quedo auditoria de una fila que nunca se guardo");
        }

        [TestMethod]
        public void LaBitacoraNoSeQuedaPegadaAlChangeTracker()
        {
            // Prueba de regresion.
            //
            // SaveChanges agrega las filas de auditoria al contexto para
            // guardarlas. Si no se sueltan despues, el contexto acumula cada
            // fila que escribio, y cualquier operacion posterior sobre esas
            // mismas filas en la misma conexion falla con "another instance
            // with the same key value is already being tracked". Eso rompia la
            // limpieza de estas pruebas, y rompe tambien Actualizar y Borrar
            // de la API de Auditorias dentro de una misma peticion.
            var etiqueta = this.nucleo.Etiqueta;

            var cargo = this.nucleo.Cargos();
            this.conexion.Cargos!.Add(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Cargos!, cargo);

            this.conexion.Cargos!.Remove(cargo);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            var pegadas = this.conexion.ChangeTracker.Entries<Auditorias>().Count();

            Assert.AreEqual(0, pegadas,
                "quedaron " + pegadas + " filas de auditoria rastreadas por " +
                "el contexto despues de guardarlas");

            // y la consecuencia practica: borrarlas desde la misma conexion
            // que las escribio tiene que funcionar
            var rastros = this.conexion.Auditorias!
                .Where(x => x.Usuario == etiqueta)
                .ToList();

            Assert.IsGreaterThan(0, rastros.Count, "no encontro rastros");

            this.conexion.Auditorias!.RemoveRange(rastros);
            this.conexion.SaveChanges();

            var quedan = this.conexion.Auditorias!.Count(x => x.Usuario == etiqueta);

            Assert.AreEqual(0, quedan,
                "no se pudieron borrar los rastros desde la misma conexion");
        }

        /// <summary>
        /// Borra los rastros de una fila concreta. Solo lo necesita la prueba
        /// del usuario anonimo, cuyos rastros quedan a nombre de 'sistema' y
        /// no de la etiqueta.
        /// </summary>
        private void Borrar(string entidad, int llave)
        {
            try
            {
                var rastros = this.conexion.Auditorias!
                    .Where(x => x.Entidad == entidad && x.Llave == llave)
                    .ToList();

                if (rastros.Count == 0)
                    return;

                this.conexion.Auditorias!.RemoveRange(rastros);
                this.conexion.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("no se pudieron borrar los rastros de " +
                    entidad + "/" + llave + ": " + ex.Message);
            }
        }
    }
}
