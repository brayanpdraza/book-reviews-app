using Dominio.Servicios.Implementaciones;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace DominioTest.Usuarios
{
    public class UsuariosTest
    {
        private readonly UsuarioBuilderTest builderTest = new UsuarioBuilderTest();
        private readonly UserValidations usuarioValidations = new UserValidations();
        [Fact]
        public void UserValidations_PasswordError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetPassword("bdfbeerberb").Build();
            string MessageError = "La contrase�a debe contener al menos una letra may�scula, una min�scula, un n�mero y un car�cter especial.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => usuarioValidations.Validate(Usuario));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }
        [Fact]
        public void UserValidations_CorreoError_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = builderTest.SetCorreo("Correomalo").Build();
            string MessageError = "Debe ingresar un correo v�lido.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => usuarioValidations.Validate(Usuario));

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
            bool esValido = usuarioValidations.Validate(Usuario);

            // Assert
            Assert.Equal(Valido, esValido);
        }
    }
}