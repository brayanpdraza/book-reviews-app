using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Usuarios.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioTest.Reviews
{
    public class ReviewsTest
    {
        private readonly ReviewBuilderTest _reviewBuilderTest = new ReviewBuilderTest();
        [Fact]
        public void reviewValidations_CalificacionMenorQueUno_ReturnsError()
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetCalificacion(0).Build();
            ReviewValidations reviewValidations = new ReviewValidations(Review);
            string MessageError = "La calificación debe estar entre 1 y 5.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => reviewValidations.Validate());
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Fact]
        public void reviewValidations_CalificacionMayorQueCinco_ReturnsError()
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetCalificacion(6).Build();
            ReviewValidations reviewValidations = new ReviewValidations(Review);
            string MessageError = "La calificación debe estar entre 1 y 5.";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => reviewValidations.Validate());
            // Assert
            Assert.Equal(MessageError, exception.Message);
        }

        [Theory]
        [InlineData("", "El comentario no puede estar vacío.")]
        [InlineData("       ", "El comentario no puede estar vacío.")]
        [InlineData(null, "El comentario no puede ser nulo.")]
        public void reviewValidations_Comentarioerror_ReturnsError(string Comentario, string MensajeError)
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.SetComentario(Comentario).Build();
            ReviewValidations reviewValidations = new ReviewValidations(Review);

            // Act
            var exception = Assert.Throws<ArgumentException>(() => reviewValidations.Validate());
            // Assert
            Assert.Equal(MensajeError, exception.Message);
        }

        //[Fact]
        //public void reviewValidations_LibroNull_ReturnsError()
        //{
        //    // Arrange
        //    LibroModelo libro = null;
        //    ReviewModel Review = _reviewBuilderTest.SetLibro(libro).Build();
        //    ReviewValidations reviewValidations = new ReviewValidations(Review);
        //    string MessageError = "El libro no puede ser nulo.";

        //    // Act
        //    var exception = Assert.Throws<ArgumentException>(() => reviewValidations.Validate());
        //    // Assert
        //    Assert.Equal(MessageError, exception.Message);
        //}

        //[Fact]
        //public void reviewValidations_UsuarioNull_ReturnsError()
        //{
        //    // Arrange
        //    ReviewModel Review = _reviewBuilderTest.SetUsuario(null).Build();
        //    ReviewValidations reviewValidations = new ReviewValidations(Review);
        //    string MessageError = "El usuario no puede ser nulo.";

        //    // Act
        //    var exception = Assert.Throws<ArgumentException>(() => reviewValidations.Validate());
        //    // Assert
        //    Assert.Equal(MessageError, exception.Message);
        //}

        [Fact]
        public void reviewValidations_ValidarCampos_ReturnsTrue()
        {
            // Arrange
            ReviewModel Review = _reviewBuilderTest.Build();
            ReviewValidations reviewValidations = new ReviewValidations(Review);
            bool Valido = true;
            // Act
            bool EsValido = reviewValidations.Validate();
            // Assert
            Assert.Equal(EsValido, Valido);
        }
    }
}
