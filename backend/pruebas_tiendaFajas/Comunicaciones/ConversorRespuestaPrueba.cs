using aplicacion_tiendaFajas.entidades;
using Newtonsoft.Json;
using System.Reflection;

namespace pruebas_tiendaFajas.Comunicaciones
{
    /// <summary>
    /// Pruebas del conversor de respuestas de Comunicaciones.
    ///
    /// Comunicaciones no deserializa el JSON del servicio tal como llega: lo
    /// pasa antes por una cadena de dieciocho Replace. Esa cadena es la pieza
    /// mas fragil de la capa, porque trabaja sobre el texto del JSON y no
    /// sobre su estructura, asi que cualquier dato que contenga uno de los
    /// patrones que busca sale alterado.
    ///
    /// Estas pruebas fijan ese comportamiento: lo que debe sobrevivir y lo que
    /// se sabe que no sobrevive. La segunda parte no es un defecto sin
    /// descubrir sino la razon documentada de dos decisiones del proyecto: los
    /// hashes de contrasena se guardan en hexadecimal y no en base64, y la
    /// columna Datos de la auditoria usa "campo=valor" y no JSON.
    ///
    /// Se llega al metodo privado por reflexion a proposito. La alternativa
    /// seria hacerlo publico solo para probarlo, lo que cambia el diseño para
    /// acomodar la prueba; o probarlo solo a traves de Ejecutar, que necesita
    /// el servicio levantado y entonces ya no seria una prueba unitaria.
    /// </summary>
    [TestClass]
    public class ConversorRespuestaPrueba
    {
        private static string Convertir(string entrada)
        {
            var metodo = typeof(presentacion_tiendaFajas.implementaciones.Comunicaciones)
                .GetMethod("Replace", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(metodo,
                "Comunicaciones ya no tiene el metodo Replace: si se renombro, " +
                "hay que actualizar esta prueba y no borrarla");

            var objeto = new presentacion_tiendaFajas.implementaciones.Comunicaciones();
            return (string)metodo!.Invoke(objeto, new object[] { entrada })!;
        }

        [TestMethod]
        public void UnaEntidadSobreviveLaConversion()
        {
            var original = new Cargos
            {
                Id = 7,
                Nombre = "Asesora comercial",
                Descripcion = "Atencion y venta de fajas",
                Salario = 1600000.00m
            };

            var respuesta = JsonConvert.SerializeObject(original);
            var convertida = Convertir(respuesta);
            var vuelta = JsonConvert.DeserializeObject<Cargos>(convertida);

            Assert.IsNotNull(vuelta, "la respuesta convertida no se pudo deserializar");
            Assert.AreEqual(original.Id, vuelta!.Id, "se perdio el Id");
            Assert.AreEqual(original.Nombre, vuelta.Nombre, "se perdio el Nombre");
            Assert.AreEqual(original.Descripcion, vuelta.Descripcion, "se perdio la Descripcion");
            Assert.AreEqual(original.Salario, vuelta.Salario, "se perdio el Salario");
        }

        [TestMethod]
        public void UnaListaSobreviveLaConversion()
        {
            var original = new List<Tallas>
            {
                new Tallas { Id = 1, Nombre = "M", Descripcion = "Cintura de 70 a 76 cm" },
                new Tallas { Id = 2, Nombre = "L", Descripcion = "Cintura de 77 a 83 cm" }
            };

            var convertida = Convertir(JsonConvert.SerializeObject(original));
            var vuelta = JsonConvert.DeserializeObject<List<Tallas>>(convertida);

            Assert.IsNotNull(vuelta, "la lista convertida no se pudo deserializar");
            Assert.HasCount(2, vuelta!, "la lista cambio de tamaño");
            Assert.AreEqual("M", vuelta[0].Nombre, "se perdio el primer elemento");
            Assert.AreEqual("L", vuelta[1].Nombre, "se perdio el segundo elemento");
        }

        [TestMethod]
        public void UnaEntidadConNavegacionSobreviveLaConversion()
        {
            // el servicio responde con Include, es decir con la entidad
            // referenciada anidada dentro de la principal
            var original = new Empleados
            {
                Id = 3,
                Cedula = "2001",
                Nombre = "Laura Gomez",
                Telefono = "3109876543",
                Sucursal = 1,
                Cargo = 1,
                _Sucursal = new Sucursales
                {
                    Id = 1,
                    Nombre = "Sucursal Centro",
                    Direccion = "Calle 52 # 49-30",
                    Ciudad = "Medellin",
                    Telefono = "6045123456",
                    Estado = true
                },
                _Cargo = new Cargos
                {
                    Id = 1,
                    Nombre = "Asesora comercial",
                    Descripcion = "Atencion y venta de fajas",
                    Salario = 1600000.00m
                }
            };

            var convertida = Convertir(JsonConvert.SerializeObject(original));
            var vuelta = JsonConvert.DeserializeObject<Empleados>(convertida);

            Assert.IsNotNull(vuelta, "la entidad anidada no se pudo deserializar");
            Assert.AreEqual("Laura Gomez", vuelta!.Nombre, "se perdio el Nombre");
            Assert.IsNotNull(vuelta._Sucursal, "se perdio la sucursal anidada");
            Assert.AreEqual("Sucursal Centro", vuelta._Sucursal!.Nombre,
                "se perdio el nombre de la sucursal anidada, que es el que " +
                "muestran las tablas del front");
            Assert.IsNotNull(vuelta._Cargo, "se perdio el cargo anidado");
            Assert.AreEqual("Asesora comercial", vuelta._Cargo!.Nombre,
                "se perdio el nombre del cargo anidado");
        }

        [TestMethod]
        public void ElTextoNullSeConvierteEnCadenaVacia()
        {
            // comportamiento conocido de la cadena de Replace: no distingue el
            // literal null del JSON de la palabra null dentro de un valor
            var salida = Convertir("{'Notas':null}");

            Assert.Contains("''", salida,
                "se esperaba que el null del JSON quedara como cadena vacia");
            Assert.DoesNotContain("null", salida,
                "la cadena de Replace debe eliminar todo null del texto");
        }

        [TestMethod]
        public void UnHashEnHexadecimalSobreviveYUnoEnBase64No()
        {
            // Esta es la prueba que sostiene la decision de nucleo/Clave:
            // guardar el hash de la contrasena en hexadecimal y no en base64.
            //
            // El alfabeto base64 incluye n, u y l, asi que un hash puede
            // contener la subcadena "null" por casualidad. Cuando eso pasa, el
            // Replace final la cambia por '' y ese usuario no podria entrar
            // nunca mas, con un error imposible de reproducir.
            //
            // El alfabeto hexadecimal es 0-9 y a-f: no puede formar "null",
            // y tampoco contiene +, / ni =, que la cadena tambien toca.
            var enBase64 = "YWJj" + "null" + "ZGVm==";
            var enHexadecimal = "573F00026DA0D364D65DDA1B31326B82.9DEAF1BFD9B3AC0E";

            Assert.AreNotEqual(enBase64, Convertir(enBase64),
                "si base64 sobreviviera, la decision de usar hexadecimal " +
                "en Clave.Derivar ya no tendria fundamento y habria que revisarla");

            Assert.AreEqual(enHexadecimal, Convertir(enHexadecimal),
                "el hash en hexadecimal debe llegar al front intacto: de el " +
                "depende que el login funcione");
        }

        [TestMethod]
        public void ElFormatoDeLaAuditoriaSobreviveLaConversion()
        {
            // la columna Datos usa "campo=valor; campo=valor" justamente para
            // no depender de llaves ni comillas, que la cadena de Replace
            // elimina. Si alguien la cambia a JSON, esta prueba lo avisa.
            var datos = "Nombre=Cargo de prueba; Salario=1450000.00; Estado=True";
            Assert.AreEqual(datos, Convertir(datos),
                "el formato de la columna Datos ya no sobrevive la conversion");
        }
    }
}
