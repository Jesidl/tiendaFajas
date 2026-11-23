using System.Security.Cryptography;

namespace aplicacion_tiendaFajas.nucleo
{
    /// <summary>
    /// Derivacion de contrasenas con PBKDF2-HMAC-SHA256.
    /// Nunca se guarda la contrasena: se guarda "sal.hash", los dos en
    /// hexadecimal.
    ///
    /// Se usa hexadecimal y no Base64 a proposito. El metodo Replace() de
    /// Comunicaciones hace, entre otros, .Replace("null", "''"), y el
    /// alfabeto de Base64 incluye las letras n, u y l, asi que un hash
    /// podria contener la subcadena "null" y llegar corrupto al otro lado.
    /// El alfabeto hexadecimal (0-9 a-f) no puede formar esa palabra, ni
    /// contiene +, / ni =, que tambien sufren en esa cadena de reemplazos.
    /// </summary>
    public class Clave
    {
        private const int TamanoSal = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 210000;

        public static string Derivar(string clave)
        {
            if (string.IsNullOrEmpty(clave))
                throw new Exception("Falta informacion");

            var sal = RandomNumberGenerator.GetBytes(TamanoSal);
            var hash = Calcular(clave, sal);

            return Convert.ToHexString(sal) + "." + Convert.ToHexString(hash);
        }

        public static bool Verificar(string? clave, string? derivada)
        {
            if (string.IsNullOrEmpty(clave) || string.IsNullOrEmpty(derivada))
                return false;

            var partes = derivada.Split('.');
            if (partes.Length != 2)
                return false;

            byte[] sal;
            byte[] esperado;
            try
            {
                sal = Convert.FromHexString(partes[0]);
                esperado = Convert.FromHexString(partes[1]);
            }
            catch (FormatException)
            {
                return false;
            }

            if (sal.Length != TamanoSal || esperado.Length != TamanoHash)
                return false;

            var obtenido = Calcular(clave, sal);

            // comparacion en tiempo constante: no filtra cuantos bytes coinciden
            return CryptographicOperations.FixedTimeEquals(obtenido, esperado);
        }

        private static byte[] Calcular(string clave, byte[] sal)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                clave,
                sal,
                Iteraciones,
                HashAlgorithmName.SHA256,
                TamanoHash);
        }
    }
}
