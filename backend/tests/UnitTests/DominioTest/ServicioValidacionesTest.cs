using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Servicios.Implementaciones;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Modelo;
using DominioTest.Reviews;
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

        [Theory]
        [InlineData(1, true)]
        [InlineData(5, true)]
        public void CalificacionLibroValidator_CalificacionPermitida_Returnstrue(int Calificacion, bool Valido)
        {
            // Arrange
            CalificacionLibroValidator Validacion = new CalificacionLibroValidator();

            // Act
            bool EsValido = Validacion.Validate(Calificacion);
            // Assert
            Assert.Equal(Valido, EsValido);
        }

        [Theory]
        [InlineData(0, "La calificación debe estar entre 1 y 5.")]
        [InlineData(6, "La calificación debe estar entre 1 y 5.")]
        public void CalificacionLibroValidator_CalificacionNoPermitida_ReturnsError(int Calificacion, string MensajeError)
        {
            // Arrange
            CalificacionLibroValidator Validacion = new CalificacionLibroValidator();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(Calificacion));
            // Assert
            Assert.Equal(MensajeError, exception.Message);
        }

        [Theory]
        [InlineData("", "El comentario no puede estar vacío.")]
        [InlineData("       ", "El comentario no puede estar vacío.")]
        [InlineData(null, "El comentario no puede ser nulo.")]
        public void ComentarioValidator_Comentarioerror_ReturnsError(string Comentario, string MensajeError)
        {
            // Arrange
            ComentarioValidator Validacion = new ComentarioValidator();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(Comentario));
            // Assert
            Assert.Equal(MensajeError, exception.Message);
        }


        [Theory]
        [InlineData("hola soy un comentario", true)]
        [InlineData("Comentario 2", true)]
        public void ComentarioValidator_Comentarioerror_ReturnsTrue(string Comentario, bool Valido)
        {
            // Arrange
            ComentarioValidator Validacion = new ComentarioValidator();

            // Act
            bool EsValido = Validacion.Validate(Comentario);
            // Assert
            Assert.Equal(Valido, EsValido);
        }


        [Fact]
        public void LibroValidator_LibroNull_ReturnsError()
        {
            // Arrange
            LibroModelo libro = null;
            LibroValidator Validacion = new LibroValidator(); 
            string MessageError = "El libro no puede ser nulo.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(libro));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Fact]
        public void LibroValidator_LibroValido_ReturnsTrue()
        {
            // Arrange
            LibroModelo libro = new LibroModelo();
            LibroValidator Validacion = new LibroValidator();
            bool Valido = true;

            // Act
            bool EsValido = Validacion.Validate(libro);
            // Assert
            Assert.Equal(Valido, EsValido);
        }

        [Fact]
        public void UsuarioValidator_UsuarioNull_ReturnsError()
        {
            // Arrange
            UsuarioModelo Usuario = null;
            UsuarioValidator Validacion = new UsuarioValidator();
            string MessageError = "El usuario no puede ser nulo.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => Validacion.Validate(Usuario));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Fact]
        public void UsuarioValidator_UsuarioValido_ReturnsTrue()
        {
            // Arrange
            UsuarioModelo Usuario = new UsuarioModelo();
            UsuarioValidator Validacion = new UsuarioValidator();
            bool Valido = true;

            // Act
            bool EsValido = Validacion.Validate(Usuario);
            // Assert
            Assert.Equal(EsValido, EsValido);
        }
    }
}
