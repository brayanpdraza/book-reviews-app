using Dominio.Servicios.Implementaciones;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace DominioTest.Usuarios
{
    public class UsuariosTest
    {
        private readonly UsuarioBuilderTest builderTest = new UsuarioBuilderTest();

        [Fact]
        public void UserValidations_PasswordError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetPassword("bdfbeerberb").Build();
            UserValidations userValidations = new UserValidations(Usuario);
            string MessageError = "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => userValidations.Validate());
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }
        [Fact]
        public void UserValidations_CorreoError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetCorreo("Correomalo").Build();
            UserValidations userValidations = new UserValidations(Usuario);
            string MessageError = "Debe ingresar un correo válido.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => userValidations.Validate());

            // Assert
            Assert.Equal(MessageError, exception.Message);
        }
        [Fact]
        public void UserValidations_ValidarCamposUsuario_ReturnsTrue()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.Build();

            UserValidations userValidations = new UserValidations(Usuario);
            bool Valido = true;

            // Act
            bool esValido = userValidations.Validate();

            // Assert
            Assert.Equal(Valido, esValido);
        }
    }
}