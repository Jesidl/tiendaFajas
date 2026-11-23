using aplicacion_tiendaFajas.entidades;
using pruebas_tiendaFajas.Nucleo;
using System.Reflection;

namespace pruebas_tiendaFajas.Comunicaciones
{
    /// <summary>
    /// Pruebas unitarias de la capa que habla con el servicio.
    ///
    /// Las veinticuatro clases XPresentacion reciben IComunicaciones por el
    /// constructor, asi que se pueden probar con un doble en lugar del
    /// transporte real: sin servicio levantado, sin base de datos y sin red.
    ///
    /// Lo que se comprueba es el contrato que el front da por supuesto:
    ///
    ///  - que cada operacion llame exactamente la URL que le corresponde. Un
    ///    error de copiado aqui (llamar "Cargos/Borrar" desde Insertar, por
    ///    ejemplo) no rompe la compilacion y en pantalla se veria como datos
    ///    que no se guardan, sin ningun mensaje.
    ///
    ///  - que las validaciones corten antes de salir a la red. Insertar algo
    ///    que ya tiene Id, o actualizar algo que no lo tiene, debe fallar en
    ///    el front y no llegar al servicio.
    ///
    ///  - que un error del transporte llegue al llamador y no se pierda.
    ///
    /// El recorrido es por reflexion para que las veinticuatro clases queden
    /// cubiertas, incluidas las que se agreguen despues: una clase nueva entra
    /// sola a la prueba. Las reglas que se verifican aqui son las unicas tres
    /// que no dependen de los campos obligatorios de cada entidad; la
    /// validacion de campos se prueba aparte, mas abajo.
    /// </summary>
    [TestClass]
    public class ContratoPresentacionPrueba
    {
        /// <summary>
        /// Las clases XPresentacion del proyecto de presentacion. Se busca por
        /// el ensamblado y no por una lista escrita a mano, que habria que
        /// recordar actualizar.
        /// </summary>
        private static List<Type> Clases()
        {
            var ensamblado =
                typeof(presentacion_tiendaFajas.implementaciones.Comunicaciones).Assembly;

            return ensamblado.GetTypes()
                .Where(x => x.IsClass
                    && !x.IsAbstract
                    && x.Name.EndsWith("Presentacion"))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private static string Entidad(Type clase)
        {
            return clase.Name.Substring(0, clase.Name.Length - "Presentacion".Length);
        }

        private static object Instancia(Type clase, ComunicacionesFalsas falsas)
        {
            var objeto = Activator.CreateInstance(clase, falsas);

            Assert.IsNotNull(objeto,
                clase.Name + " no se pudo construir con un IComunicaciones: " +
                "la inyeccion por constructor es la que permite probar esta capa");

            return objeto!;
        }

        /// <summary>
        /// Invoca por reflexion y desenvuelve la excepcion real, que la
        /// reflexion entrega envuelta en TargetInvocationException.
        /// </summary>
        private static Exception? Invocar(object objeto, string metodo, object?[]? argumentos)
        {
            try
            {
                objeto.GetType().GetMethod(metodo)!.Invoke(objeto, argumentos);
                return null;
            }
            catch (TargetInvocationException ex)
            {
                return ex.InnerException ?? ex;
            }
        }

        [TestMethod]
        public void HayVeinticuatroClasesDePresentacion()
        {
            var clases = Clases();

            Assert.HasCount(24, clases,
                "se esperaba una clase de presentacion por entidad; se " +
                "encontraron: " + string.Join(", ", clases.Select(x => x.Name)));
        }

        [TestMethod]
        public void CadaClaseCorrespondeASuEntidad()
        {
            foreach (var clase in Clases())
            {
                var parametro = clase.GetMethod("Buscar")!.GetParameters()[0].ParameterType;

                Assert.AreEqual(Entidad(clase), parametro.Name,
                    clase.Name + " recibe " + parametro.Name + ": el nombre de " +
                    "la clase y el de la entidad no coinciden, asi que la URL " +
                    "que arma no puede ser la correcta");
            }
        }

        [TestMethod]
        public void ConsultarLlamaSuPropiaUrl()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);

                clase.GetMethod("Consultar")!.Invoke(objeto, null);

                Assert.AreEqual(Entidad(clase) + "/Consultar", falsas.Url,
                    clase.Name + ".Consultar llamo la URL equivocada");
                Assert.AreEqual(1, falsas.Llamadas,
                    clase.Name + ".Consultar debe hacer una sola llamada");
            }
        }

        [TestMethod]
        public void BuscarLlamaSuPropiaUrl()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);
                var entidad = clase.GetMethod("Buscar")!.GetParameters()[0].ParameterType;

                clase.GetMethod("Buscar")!.Invoke(objeto,
                    new object?[] { Activator.CreateInstance(entidad) });

                Assert.AreEqual(Entidad(clase) + "/Buscar", falsas.Url,
                    clase.Name + ".Buscar llamo la URL equivocada");
            }
        }

        [TestMethod]
        public void BorrarLlamaSuPropiaUrl()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);

                // Borrar solo exige que la fila ya tenga Id, no revisa campos
                var entidad = clase.GetMethod("Borrar")!.GetParameters()[0].ParameterType;
                var fila = Activator.CreateInstance(entidad)!;
                entidad.GetProperty("Id")!.SetValue(fila, 1);

                clase.GetMethod("Borrar")!.Invoke(objeto, new object?[] { fila });

                Assert.AreEqual(Entidad(clase) + "/Borrar", falsas.Url,
                    clase.Name + ".Borrar llamo la URL equivocada");
            }
        }

        [TestMethod]
        public void NoSeInsertaAlgoQueYaTieneId()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);

                var entidad = clase.GetMethod("Insertar")!.GetParameters()[0].ParameterType;
                var fila = Activator.CreateInstance(entidad)!;
                entidad.GetProperty("Id")!.SetValue(fila, 1);

                var error = Invocar(objeto, "Insertar", new object?[] { fila });

                Assert.IsNotNull(error,
                    clase.Name + ".Insertar acepto una fila que ya tiene Id: " +
                    "eso duplicaria el registro en la base");
                Assert.AreEqual(0, falsas.Llamadas,
                    clase.Name + ".Insertar debe cortar antes de salir a la red");
            }
        }

        [TestMethod]
        public void NoSeActualizaAlgoSinId()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);

                var entidad = clase.GetMethod("Actualizar")!.GetParameters()[0].ParameterType;
                var fila = Activator.CreateInstance(entidad)!;

                var error = Invocar(objeto, "Actualizar", new object?[] { fila });

                Assert.IsNotNull(error,
                    clase.Name + ".Actualizar acepto una fila sin Id: no hay " +
                    "nada que actualizar en la base");
                Assert.AreEqual(0, falsas.Llamadas,
                    clase.Name + ".Actualizar debe cortar antes de salir a la red");
            }
        }

        [TestMethod]
        public void NoSeBorraAlgoSinId()
        {
            foreach (var clase in Clases())
            {
                var falsas = new ComunicacionesFalsas();
                var objeto = Instancia(clase, falsas);

                var entidad = clase.GetMethod("Borrar")!.GetParameters()[0].ParameterType;
                var fila = Activator.CreateInstance(entidad)!;

                var error = Invocar(objeto, "Borrar", new object?[] { fila });

                Assert.IsNotNull(error,
                    clase.Name + ".Borrar acepto una fila sin Id");
                Assert.AreEqual(0, falsas.Llamadas,
                    clase.Name + ".Borrar debe cortar antes de salir a la red");
            }
        }

        // ------------------------------------------------------------------
        // Validacion de campos obligatorios y propagacion de errores. Estas
        // dependen de los campos de cada entidad, asi que se escriben sobre
        // casos concretos en lugar de recorrer las veinticuatro clases.
        // ------------------------------------------------------------------

        [TestMethod]
        public void NoSeInsertaUnCargoSinDescripcion()
        {
            var falsas = new ComunicacionesFalsas();
            var presentacion = new aplicacion_tiendaFajas.implementaciones
                .CargosPresentacion(falsas);

            Exception? error = null;
            try
            {
                presentacion.Insertar(new Cargos { Nombre = "Bodeguero" });
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.IsNotNull(error, "acepto un Cargos sin Descripcion");
            Assert.AreEqual("Falta informacion", error!.Message,
                "se esperaba el mensaje de validacion de la capa de presentacion");
            Assert.AreEqual(0, falsas.Llamadas,
                "una fila incompleta no debe llegar al servicio");
        }

        [TestMethod]
        public void SeInsertaUnCargoCompleto()
        {
            var falsas = new ComunicacionesFalsas();
            var presentacion = new aplicacion_tiendaFajas.implementaciones
                .CargosPresentacion(falsas);

            var resultado = presentacion.Insertar(new Cargos
            {
                Nombre = "Bodeguero",
                Descripcion = "Recepcion y control de mercancia",
                Salario = 1450000.00m
            });

            Assert.AreEqual("Cargos/Insertar", falsas.Url, "URL equivocada");
            Assert.IsNotNull(resultado, "no devolvio la entidad");
            Assert.AreEqual("Bodeguero", resultado.Nombre,
                "la entidad no viajo completa hasta el transporte");

            var enviado = falsas.Enviado as Cargos;
            Assert.IsNotNull(enviado, "lo que se envio no es un Cargos");
            Assert.AreEqual(1450000.00m, enviado!.Salario,
                "el Salario no llego al transporte");
        }

        [TestMethod]
        public void SeConsultaUnaListaDeCargos()
        {
            var falsas = new ComunicacionesFalsas();
            falsas.Respuesta = new List<Cargos>
            {
                new Cargos { Id = 1, Nombre = "Asesora comercial" },
                new Cargos { Id = 2, Nombre = "Bodeguero" }
            };

            var presentacion = new aplicacion_tiendaFajas.implementaciones
                .CargosPresentacion(falsas);

            var lista = presentacion.Consultar();

            Assert.AreEqual("Cargos/Consultar", falsas.Url, "URL equivocada");
            Assert.HasCount(2, lista, "no devolvio las dos filas");
            Assert.AreEqual("Bodeguero", lista[1].Nombre, "se perdio la segunda fila");
        }

        [TestMethod]
        public void UnErrorDelTransporteLlegaAlLlamador()
        {
            // si el servicio esta caido, el front tiene que enterarse: un
            // error que se pierde aqui se ve en pantalla como una tabla vacia
            var falsas = new ComunicacionesFalsas();
            falsas.Falla = new Exception("Error Comunicacion");

            var presentacion = new aplicacion_tiendaFajas.implementaciones
                .CargosPresentacion(falsas);

            Exception? error = null;
            try
            {
                presentacion.Consultar();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.IsNotNull(error,
                "la capa de presentacion se trago el error del transporte");

            // task.Wait() envuelve el error en AggregateException: lo que se
            // comprueba es que el mensaje original sigue siendo recuperable
            var envuelto = error as AggregateException;
            var mensaje = envuelto != null
                ? envuelto.InnerException!.Message
                : error!.Message;

            Assert.AreEqual("Error Comunicacion", mensaje,
                "el error del transporte llego cambiado o vacio");
        }
    }
}
