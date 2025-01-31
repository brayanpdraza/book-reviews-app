using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Servicios.Implementaciones;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;

namespace DominioTest.Usuarios
{
    public class UsuariosTest
    {
        private readonly IUserValidations _userValidations = new UserValidations();

        private readonly UsuarioBuilderTest builderTest;

        public UsuariosTest()
        {
            builderTest = new UsuarioBuilderTest(_userValidations);
        }

        [Fact]
        public void UserValidations_PasswordError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetPassword("bdfbeerberb").Build();
            string MessageError = "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _userValidations.Validate(Usuario));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }
        [Fact]
        public void UserValidations_CorreoError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetCorreo("Correomalo").Build();
            string MessageError = "Debe ingresar un correo válido.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _userValidations.Validate(Usuario));

            // Assert
            Assert.Equal(MessageError, exception.Message);
        }
        [Fact]
        public void UserValidations_ValidarCamposUsuario_ReturnsTrue()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.Build();

            bool Valido = true;

            // Act
            bool esValido = _userValidations.Validate(Usuario);

            // Assert
            Assert.Equal(Valido, esValido);
        }
    }
}