using Aplicacion.Reviews;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionTest.Reviews
{
    public class ReviewsUseCaseTest
    {
        private readonly Mock<IReviewRepositorio> _mockReviewRepositorio;
        private readonly Mock<ILibroRepositorio> _mockLibroRepositorio;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private readonly Mock<IReviewValidations> _mockReviewValidations;

        private readonly UseCaseReview _useCaseReview;
        private readonly ReviewsBuilderCaseTest _reviewBuilderCaseTest;

        public ReviewsUseCaseTest()
        {
            _mockReviewRepositorio = new Mock<IReviewRepositorio>();
            _mockLibroRepositorio = new Mock<ILibroRepositorio>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockReviewValidations = new Mock<IReviewValidations>();
            _useCaseReview = new UseCaseReview(
                _mockReviewRepositorio.Object,
                _mockLibroRepositorio.Object,
                _mockUsuarioRepositorio.Object,
                _mockReviewValidations.Object
            );
            _reviewBuilderCaseTest = new ReviewsBuilderCaseTest(_mockReviewValidations.Object);
        }

        [Fact]
        public void AddReview_UsuarioNoRegistrado_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idNoRegistrado = 10;
            review.Usuario.Id = idNoRegistrado;
            string ErrorMessage = "El Usuario que intenta realizar la reseña no se encuentra registrado en el sistema.";

            _mockReviewValidations.Setup(u => u.Validate(review)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(review.Usuario.Id)).Returns(new UsuarioModelo());

            // Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.AddReview(review));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockReviewValidations.Verify(r => r.Validate(review), Times.Once);
            _mockUsuarioRepositorio.Verify(e => e.ListUsuarioPorId(review.Usuario.Id), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Never);
            _mockReviewRepositorio.Verify(r => r.AddReview(review), Times.Never);
        }

        [Fact]
        public void AddReview_LibroNoRegistrado_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idNoRegistrado = 10;
            review.Libro.Id = idNoRegistrado;
            string ErrorMessage = "El libro al que intenta realizar la reseña no se encuentra registrado en el sistema.";

            _mockReviewValidations.Setup(u => u.Validate(review)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(review.Usuario.Id)).Returns(review.Usuario);
            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(review.Libro.Id)).Returns(new LibroModelo());

            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.AddReview(review));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockReviewValidations.Verify(r => r.Validate(review), Times.Once);
            _mockUsuarioRepositorio.Verify(e => e.ListUsuarioPorId(review.Usuario.Id), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.AddReview(review), Times.Never);
        }

        [Fact]
        public void AddReview_ReviewNoAñadida_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idNoCreado = 0;
            string ErrorMessage = "La reseña no ha sido creada.";

            _mockReviewValidations.Setup(u => u.Validate(review)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(review.Usuario.Id)).Returns(review.Usuario);
            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(review.Libro.Id)).Returns(review.Libro);
            _mockReviewRepositorio.Setup(r => r.AddReview(review)).Returns(idNoCreado);

            // Act 
            var exception = Assert.Throws<Exception>(() => _useCaseReview.AddReview(review));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockReviewValidations.Verify(r => r.Validate(review), Times.Once);
            _mockUsuarioRepositorio.Verify(e => e.ListUsuarioPorId(review.Usuario.Id), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.AddReview(review), Times.Once);
        }

        [Fact]
        public void AddReview_ReviewAñadida_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idCreado = 1;

            _mockReviewValidations.Setup(u => u.Validate(review)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(review.Usuario.Id)).Returns(review.Usuario);
            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(review.Libro.Id)).Returns(review.Libro);
            _mockReviewRepositorio.Setup(r => r.AddReview(review)).Returns(idCreado);

            // Act 
            long idResultado =  _useCaseReview.AddReview(review);

            //Assert
            Assert.Equal(idCreado, idResultado);

            _mockReviewValidations.Verify(r => r.Validate(review), Times.Once);
            _mockUsuarioRepositorio.Verify(e => e.ListUsuarioPorId(review.Usuario.Id), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.AddReview(review), Times.Once);
        }

        [Fact]
        public void ConsultarReviewsPorLibro_LibroNull_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            LibroModelo libro = null;
            string ErrorMessage = "No se pueden consultar reseñas porque el libro proporcionado es nulo.";

            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorLibro(libro));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorLibro(libro), Times.Never);
        }


        [Fact]
        public void ConsultarReviewsPorLibro_LibroidNoValido_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            LibroModelo libro = review.Libro;
            long idNoValido = 0;
            libro.Id = idNoValido;
            string ErrorMessage = "No se pueden consultar las reseñas porque el ID del libro no es válido.";

            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorLibro(libro));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorLibro(libro), Times.Never);
        }
        [Fact]
        public void ConsultarReviewsPorLibro_LibroNoCreado_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            LibroModelo libro = review.Libro;
            long idNoCreado = 10;
            libro.Id = idNoCreado;
            string ErrorMessage = "El libro al que intenta Consultar sus reseñas no se encuentra registrado en el sistema.";

            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(review.Libro.Id)).Returns(new LibroModelo());
            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorLibro(libro));


            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorLibro(libro), Times.Never);

        }

        [Fact]
        public void ConsultarReviewsPorLibro_ReseñasExistentes_LanzaExcepcion()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            LibroModelo libro = review.Libro;
            List<ReviewModel> LisReviewsConsultads = new List<ReviewModel> { review};

            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(libro.Id)).Returns(libro);
            _mockReviewRepositorio.Setup(r => r.ListReviewPorLibro(libro)).Returns(LisReviewsConsultads );

            // Act 
            List<ReviewModel> listReviewResultado = _useCaseReview.ConsultarReviewsPorLibro(libro);

            //Assert
            Assert.Equal(LisReviewsConsultads, listReviewResultado);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorLibro(libro), Times.Once);
        }
    }
}
