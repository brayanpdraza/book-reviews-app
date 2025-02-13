using Dominio.Entidades.Reviews.Puertos;
using Dominio.Entidades.Reviews.Servicios;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Servicios.ServicioValidaciones.Contratos;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Servicios;
using DominioTest.Usuarios;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioTest.Reviews
{
    public class ReviewsTest
    {
        private readonly IReviewValidations _reviewValidations = new ReviewValidations();
        private readonly IReviewPartialUpdateValidations _reviewUpdateValidations = new ReviewPartialUpdateValidations();
        private readonly ReviewBuilderTest _reviewBuilderTest;
        public ReviewsTest()
        {
            _reviewBuilderTest = new ReviewBuilderTest(_reviewValidations);
        }

        [Theory]
        [InlineData(0, "La calificación debe estar entre 1 y 5.")]
        [InlineData(6, "La calificación debe estar entre 1 y 5.")]
        public void reviewValidations_CalificacionNoPermitida_ReturnsError(int Calificacion,string MensajeError)
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetCalificacion(Calificacion).Build();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _reviewValidations.Validate(Review));
            // Assert
            Assert.Equal(MensajeError, exception.Message);
        }



        [Theory]
        [InlineData("", "El comentario no puede estar vacío.")]
        [InlineData("       ", "El comentario no puede estar vacío.")]
        [InlineData(null, "El comentario no puede ser nulo.")]
        public void reviewValidations_Comentarioerror_ReturnsError(string Comentario, string MensajeError)
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetComentario(Comentario).Build();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _reviewValidations.Validate(Review));
            // Assert
            Assert.Equal(MensajeError, exception.Message);
        }

        [Fact]
        public void reviewValidations_LibroNull_ReturnsError()
        {
            // Arrange
            LibroModelo libro = null;
            ReviewModel Review = _reviewBuilderTest.SetLibro(libro).Build();
            string MessageError = "El libro no puede ser nulo.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _reviewValidations.Validate(Review));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Fact]
        public void reviewValidations_UsuarioNull_ReturnsError()
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetUsuario(null).Build();
            string MessageError = "El usuario no puede ser nulo.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _reviewValidations.Validate(Review));
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Fact]
        public void reviewValidations_ValidarCampos_ReturnsTrue()
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.Build();
            bool Valido = true;
            // Act
            bool EsValido = _reviewValidations.Validate(Review);
            // Assert
            Assert.Equal(EsValido, Valido);
        }

        [Theory]
        [InlineData("AUTOR", "Nuevo Autor")] // Campo no permitido
        [InlineData("ID", 2)] // Campo inexistente
        public void reviewValidationsUpdate_InvalidKey_ThrowsException(string key, object value)
        {
            // Act
            var exception = Assert.Throws<Exception>(() => _reviewUpdateValidations.Validate(key, value));

            //Assert
            Assert.Contains($"El campo '{key}' no está permitido para modificación.", exception.Message);
        }

        /*[Theory]
        [InlineData("CALIFICACION", 1)] // Calificación fuera de rango
        [InlineData("CALIFICACION", 3)] // Supongamos que el máximo es 5
        [InlineData("COMENTARIO", "")] // Comentario vacío no válido
        public void reviewValidationsUpdate_InvalidFields_ReturnsFalse(string key, object value)
        {
            // Act
            bool result = _reviewUpdateValidations.Validate(key, value);

            // Assert
            Assert.False(result);
        }
        */

        [Theory]
        [InlineData("CALIFICACION", 5)] // Calificación válida
        [InlineData("COMENTARIO", "Buen libro")] // Comentario válido
        public void reviewValidationsUpdate_ValidFields_ReturnsTrue(string key, object value)
        {
            // Act
            bool result = _reviewUpdateValidations.Validate(key, value);

            // Assert
            Assert.True(result);
        }


    }
}
