using Dominio.Servicios.Implementaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioTest
{
    public class ServicioValidacionesTest
    {
        [Theory]
        [InlineData("", "Debe ingresar una contraseña!")]
        [InlineData("123aS@", "La contraseña debe tener al menos 8 caracteres.")]
        [InlineData("hrthhrthraS@", "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        [InlineData("123aSjtyj6j56", "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        [InlineData("123af@fqfqfq", "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        public void PasswordValidator_ValidacionesError_ReturnsError(string InvalidPass, string MessageError)
        {
            // Arrange
            PasswordValidator Validacion = new PasswordValidator();
            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(InvalidPass));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Theory]
        [InlineData("Goodp4ss*", true)]
        [InlineData("anOth3rp@ss", true)]
        public void PasswordValidator_PasswordCorrecta_ReturnsTrue(string Pass, bool CorrectPass)
        {
            // Arrange
            PasswordValidator Validacion = new PasswordValidator();
            bool expectedCorrectpass = false;
            // Act
            expectedCorrectpass = Validacion.Validate(Pass);
            // Assert
            Assert.Equal(CorrectPass, expectedCorrectpass);
        }

        [Theory]
        [InlineData("", "Debe ingresar un correo!")]
        [InlineData("correo", "Debe ingresar un correo válido.")]
        [InlineData("correo@", "Debe ingresar un correo válido.")]
        [InlineData("correo@invalido", "Debe ingresar un correo válido.")]
        public void EmailValidator_ValidacionesError_ReturnsError(string InvalidCorreo, string MessageError)
        {
            // Arrange
            EmailValidator Validacion = new EmailValidator();
            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(InvalidCorreo));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Theory]
        [InlineData("correo@valido.com", true)]
        [InlineData("brayanelarquero@gmail.com", true)]
        public void EmailValidator_CorreoCorrecto_ReturnsTrue(string Correo, bool CorrectCorreo)
        {
            // Arrange
            EmailValidator Validacion = new EmailValidator();
            bool expectedCorrectCorreo = false;
            // Act
            expectedCorrectCorreo = Validacion.Validate(Correo);
            // Assert
            Assert.Equal(CorrectCorreo, expectedCorrectCorreo);
        }
    }
}
