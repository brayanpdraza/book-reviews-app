using AdaptadorEncripter;
using Dominio.Servicios.ServicioEncripcion.Contratos;

namespace AdaptadorEncriptacionTest
{
    public class PruebasEncriptacion
    {

        private readonly IEncription _encription;

        public PruebasEncriptacion()
        {
            _encription = new PBKDF2Encription();
        }


        [Fact]
        public void Encriptar_SamePassword_ReturnsNotSameHash()
        {
            // Arrange
            string password = "Password123";

            // Act
            string hash1 = _encription.Encriptar(password);
            string hash2 = _encription.Encriptar(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // Porque el salt es aleatorio
        }

        [Fact]
        public void Encriptar_DifferentPasswords_ReturnsDifferentHashes()
        {
            // Arrange
            string password1 = "Password123";
            string password2 = "Password456";

            // Act
            string hash1 = _encription.Encriptar(password1);
            string hash2 = _encription.Encriptar(password2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void Encriptar_HashHasCorrectLength()
        {
            // Arrange
            string password = "Password123";

            // Act
            string hash = _encription.Encriptar(password);

            // Assert
            Assert.Equal(48, hash.Length); // 36 bytes en base64 es igual a 48 caracteres
        }
        [Theory]
        [InlineData("Password123", true)]
        [InlineData("ContraseñaSegura", true)]
        public void VerificarClaveEncriptada_CorrectPassword_ReturnsTrue(string contraseñaOriginal, bool ExpectedValue)
        {
            // Arrange
            string hash = _encription.Encriptar(contraseñaOriginal);

            // Act
            bool resultado = _encription.VerificarClaveEncriptada(contraseñaOriginal, hash);

            // Assert
            Assert.Equal(resultado, ExpectedValue);
        }

    }
}