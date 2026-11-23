using aplicacion_tiendaFajas.nucleo;
using System.Text.RegularExpressions;

namespace pruebas_tiendaFajas.Dominio
{
    /// <summary>
    /// Pruebas de la derivacion de contraseñas.
    ///
    /// Hasta ahora esto solo estaba probado de forma indirecta, a traves del
    /// login. Un fallo aqui se veria como "no puedo entrar" y se buscaria en
    /// el front, en la sesion o en la consulta, que es donde no esta.
    /// </summary>
    [TestClass]
    public class ClavePrueba
    {
        private const string Secreto = "Admin.123";

        [TestMethod]
        public void LaClaveDerivadaNoContieneLaContrasena()
        {
            var derivada = Clave.Derivar(Secreto);

            Assert.DoesNotContain(Secreto, derivada,
                "la contrasena esta apareciendo en el texto guardado");
            Assert.IsGreaterThan(64, derivada.Length,
                "el resultado es demasiado corto para ser sal mas hash");
        }

        [TestMethod]
        public void ElFormatoEsSalPuntoHashEnHexadecimal()
        {
            // 16 bytes de sal y 32 de hash, en hexadecimal: 32 y 64 caracteres
            var esperado = new Regex("^[0-9A-Fa-f]{32}\\.[0-9A-Fa-f]{64}$");

            Assert.IsTrue(esperado.IsMatch(Clave.Derivar(Secreto)),
                "el formato cambio. Es hexadecimal y no base64 a proposito: el " +
                "alfabeto base64 incluye n, u y l, y la cadena de Replace del " +
                "cliente HTTP sustituye 'null' por una cadena vacia, asi que un " +
                "hash en base64 podria llegar corrupto");
        }

        [TestMethod]
        public void LaMismaContrasenaDaResultadosDistintos()
        {
            // la sal es aleatoria por derivacion. Si dos usuarios con la misma
            // contrasena tuvieran el mismo hash, una tabla precalculada
            // resolveria las dos de un golpe.
            var primera = Clave.Derivar(Secreto);
            var segunda = Clave.Derivar(Secreto);

            Assert.AreNotEqual(primera, segunda,
                "la sal no esta siendo aleatoria");

            // y las dos tienen que verificar
            Assert.IsTrue(Clave.Verificar(Secreto, primera), "fallo la primera");
            Assert.IsTrue(Clave.Verificar(Secreto, segunda), "fallo la segunda");
        }

        [TestMethod]
        public void VerificaLaContrasenaCorrectaYRechazaLasDemas()
        {
            var derivada = Clave.Derivar(Secreto);

            Assert.IsTrue(Clave.Verificar(Secreto, derivada),
                "rechazo la contrasena correcta");

            Assert.IsFalse(Clave.Verificar("Admin.124", derivada), "acepto una distinta");
            Assert.IsFalse(Clave.Verificar("admin.123", derivada),
                "acepto la misma con otras mayusculas");
            Assert.IsFalse(Clave.Verificar("Admin.123 ", derivada),
                "acepto la misma con un espacio al final");
            Assert.IsFalse(Clave.Verificar("", derivada), "acepto una vacia");
            Assert.IsFalse(Clave.Verificar(null, derivada), "acepto null");
        }

        [TestMethod]
        public void UnHashAlteradoNoVerifica()
        {
            var derivada = Clave.Derivar(Secreto);

            // se cambia el ultimo caracter del hash
            var ultimo = derivada[derivada.Length - 1];
            var otro = ultimo == '0' ? '1' : '0';
            var alterado = derivada.Substring(0, derivada.Length - 1) + otro;

            Assert.IsFalse(Clave.Verificar(Secreto, alterado),
                "un hash manipulado en un solo caracter sigue verificando");
        }

        [TestMethod]
        public void UnValorGuardadoInvalidoNoVerifica()
        {
            // si alguien escribio la contrasena en claro en la tabla, o el
            // campo quedo a medias, no debe poder entrar nadie
            Assert.IsFalse(Clave.Verificar(Secreto, Secreto),
                "acepto la contrasena en texto plano como si fuera el hash");
            Assert.IsFalse(Clave.Verificar(Secreto, ""), "acepto un hash vacio");
            Assert.IsFalse(Clave.Verificar(Secreto, null), "acepto un hash null");
            Assert.IsFalse(Clave.Verificar(Secreto, "sinpunto"), "acepto un valor sin punto");
            Assert.IsFalse(Clave.Verificar(Secreto, "zz.zz"), "acepto algo que no es hexadecimal");
        }
    }
}
