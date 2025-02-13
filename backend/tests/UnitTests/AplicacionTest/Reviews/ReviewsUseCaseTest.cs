using Aplicacion.Methods;
using Aplicacion.Reviews;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using Dominio.Servicios.ServicioValidaciones.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit.Sdk;

namespace AplicacionTest.Reviews
{
    public class ReviewsUseCaseTest
    {
        private readonly Mock<IReviewRepositorio> _mockReviewRepositorio;
        private readonly Mock<ILibroRepositorio> _mockLibroRepositorio;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private readonly Mock<IReviewValidations> _mockReviewValidations;
        private readonly Mock<IReviewPartialUpdateValidations> _mockReviewPartialUpdateValidations;
        private readonly Mock<IpropertyModelValidate> _mockPropertyModelValidate;

        private readonly MetodosAuxiliares _metodosAuxiliares;
        private readonly UseCaseReview _useCaseReview;
        private readonly ReviewsBuilderCaseTest _reviewBuilderCaseTest;

        public ReviewsUseCaseTest()
        {
            _mockReviewRepositorio = new Mock<IReviewRepositorio>();
            _mockLibroRepositorio = new Mock<ILibroRepositorio>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockReviewValidations = new Mock<IReviewValidations>();
            _mockReviewPartialUpdateValidations = new Mock<IReviewPartialUpdateValidations>();
            _mockPropertyModelValidate = new Mock<IpropertyModelValidate>();
            _metodosAuxiliares = new MetodosAuxiliares();
            _useCaseReview = new UseCaseReview(
                _mockReviewRepositorio.Object,
                _mockLibroRepositorio.Object,
                _mockUsuarioRepositorio.Object,
                _mockReviewValidations.Object,
                _mockReviewPartialUpdateValidations.Object,
                _mockPropertyModelValidate.Object,
                _metodosAuxiliares
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
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseReview.AddReview(review));

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
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseReview.AddReview(review));

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
            long idResultado = _useCaseReview.AddReview(review);

            //Assert
            Assert.Equal(idCreado, idResultado);

            _mockReviewValidations.Verify(r => r.Validate(review), Times.Once);
            _mockUsuarioRepositorio.Verify(e => e.ListUsuarioPorId(review.Usuario.Id), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.AddReview(review), Times.Once);
        }

        /*[Theory]
        [InlineData(2, 1)]
        public void ConsultarReviewsPorUsuarioPaginados_UsuarioNull_LanzaExcepcion(int Pagina, int tamanoPagina)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = null;
            string ErrorMessage = "No se pueden consultar reseñas porque el usuario proporcionado es nulo.";

            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorUsuarioPaginados(usuario,Pagina,tamanoPagina));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(review.Usuario.Id), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(usuario,Pagina,tamanoPagina), Times.Never);
        }*/

        [Theory]
        [InlineData(2, 1)]
        public void ConsultarReviewsPorUsuarioPaginados_UsuarioidNoValido_LanzaExcepcion(int Pagina, int tamanoPagina)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idNoValido = 0;
            string ErrorMessage = "No se pueden consultar las reseñas porque el ID del usuario no es válido.";

            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idNoValido, Pagina, tamanoPagina));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(review.Usuario.Id), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(usuario, Pagina, tamanoPagina), Times.Never);
        }


        [Theory]
        [InlineData(0, 1, "La página debe ser mayor a cero.")]
        [InlineData(1, 0, "El tamaño de página debe ser mayor a cero.")]
        public void ConsultarReviewsPorUsuarioPaginados_Validacionpaginas_LanzaExcepcion(int Pagina, int tamanoPagina, string MessageError)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idUsuario = usuario.Id;
            //Act

            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idUsuario, Pagina, tamanoPagina));

            // Assert
            Assert.Equal(MessageError, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(It.IsAny<long>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(usuario, Pagina, tamanoPagina), Times.Never);
        }

        [Theory]
        [InlineData(2, 1)]
        public void ConsultarReviewsPorUsuarioPaginados_UsuarioNoCreado_LanzaExcepcion(int Pagina, int tamanoPagina)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idNoCreado = 10;
            string ErrorMessage = "El usuario al que intenta consultar sus reviews, no se encuentra en el sistema.";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(idNoCreado)).Returns(new UsuarioModelo());
            // Act 
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idNoCreado, Pagina, tamanoPagina));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(It.IsAny<long>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(usuario, Pagina, tamanoPagina), Times.Never);

        }

        [Theory]
        [InlineData(2, 1, 0)]
        public void ConsultarReviewsPorUsuarioPaginados_ReturnsEmptyLibros(int Pagina, int tamanoPagina, int TotalReviews)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idUsuario = usuario.Id;
            List<ReviewModel> ReviewVacio = new List<ReviewModel>();
            int PaginaResult = 1;

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(idUsuario)).Returns(usuario);
            _mockReviewRepositorio.Setup(r => r.ConteoDistinctLibrosReviewsPorUsuario(usuario)).Returns(TotalReviews);

            //Act
            PaginacionResultadoModelo<ReviewModel> Resultado = _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idUsuario, Pagina, tamanoPagina);

            // Assert
            Assert.Equal(ReviewVacio, Resultado.Items);
            Assert.Equal(PaginaResult, Resultado.PaginaActual);
            Assert.Equal(tamanoPagina, Resultado.TamanoPagina);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(It.IsAny<long>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(It.IsAny<UsuarioModelo>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }


        [Theory]
        [InlineData(4, 10, 29)]
        public void ConsultarReviewsPorUsuarioPaginados_PaginaExcedeTotalPaginas_LanzaExcepcion(int Pagina, int tamanoPagina, int TotalReviews)
        {  
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idUsuario = usuario.Id;

            string MessageError = $"La página solicitada ({Pagina}) excede el total de páginas disponibles.";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(idUsuario)).Returns(usuario);
            _mockReviewRepositorio.Setup(r => r.ConteoDistinctLibrosReviewsPorUsuario(usuario)).Returns(TotalReviews);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idUsuario, Pagina, tamanoPagina));

            // Assert
            Assert.Equal(MessageError, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(It.IsAny<long>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(It.IsAny<UsuarioModelo>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }


        [Theory]
        [InlineData(3, 10, 50, "")]
        public void ConsultarReviewsPorUsuarioPaginados_ValidData_ReturnsReviewPaginados(int Pagina, int tamanoPagina, int TotalReviews, string Filtro)
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            UsuarioModelo usuario = review.Usuario;
            long idUsuario = usuario.Id;
            List<ReviewModel> Reviews = new List<ReviewModel> { _reviewBuilderCaseTest.Build() };

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(idUsuario)).Returns(usuario);
            _mockReviewRepositorio.Setup(r => r.ConteoDistinctLibrosReviewsPorUsuario(usuario)).Returns(TotalReviews);
            _mockReviewRepositorio.Setup(r => r.ListReviewPorUsuarioPaginado(It.IsAny<UsuarioModelo>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Reviews);

            //Act
            PaginacionResultadoModelo<ReviewModel> Resultado = _useCaseReview.ConsultarReviewsPorUsuarioPaginados(idUsuario, Pagina, tamanoPagina);

            // Assert
            Assert.Equal(Reviews, Resultado.Items);
            Assert.Equal(Pagina, Resultado.PaginaActual);
            Assert.Equal(tamanoPagina, Resultado.TamanoPagina);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(It.IsAny<long>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ConteoDistinctLibrosReviewsPorUsuario(It.IsAny<UsuarioModelo>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorUsuarioPaginado(It.IsAny<UsuarioModelo>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseReview.ConsultarReviewsPorLibro(libro));


            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(review.Libro.Id), Times.Once);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorLibro(libro), Times.Never);

        }

        [Fact]
        public void ConsultarReviewsPorLibro_ReseñasExistentes_ReturnsReview()
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


        [Theory]
        [InlineData(0, "No se puede consultar la reseña porque el id no es válido.")]
        public void ConsultarReviewsPoriD_idErrores_LanzaExcepcion(long id, string ErrorMessage)
        {
            // Arrange


            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ConsultarReviewsPoriD(id));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(id), Times.Never);
        }


        [Fact]
        public void ConsultarReviewsPoriD_ReseñasExistentes_ReturnsReview()
        {
            // Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idExistente = review.Id;

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idExistente)).Returns(review);

            // Act 
            ReviewModel ReviewResultado = _useCaseReview.ConsultarReviewsPoriD(idExistente);

            //Assert
            Assert.Equal(review, ReviewResultado);

            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(idExistente), Times.Once);
        }

        [Fact]
        public void ModificarReviewsPorId_ReseñaconIdIncorrecto_ReturnsError()
        {
            //Arrange
            long idInvalido = 0;
            Dictionary<string,object> cambios = new Dictionary<string, object>();
            string ErrorMessage = "No se puede modificar la reseña porque el ID no es válido.";
            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ModificarReviewPorId(idInvalido,cambios));
            //Assert

            Assert.Equal(ErrorMessage, exception.Message);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Never);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Never);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Never);

        }

        [Fact]
        public void ModificarReviewsPorId_ReseñaconIdNoexistente_ReturnsError()
        {
            //Arrange
            long idNoExistente = 10;
            Dictionary<string, object> cambios = new Dictionary<string, object>();
            string ErrorMessage = "Review no encontrada.";

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idNoExistente)).Returns(new ReviewModel());
            //Act
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseReview.ModificarReviewPorId(idNoExistente, cambios));
            //Assert

            Assert.Equal(ErrorMessage, exception.Message);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Once);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Never);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Never);

        }


        [Theory]
        [InlineData("HOLA", "fsdf")]
        [InlineData("NOEXISTO", 352)]
        public void ModificarReviewsPorId_ReseñaconCampoNoExistente_ReturnsError(string key, object value)
        {
            //Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idExistente = review.Id;
            Dictionary<string, object> cambios = new Dictionary<string, object>()
            {
                { key, value }
            };
            string ErrorMessage = $"El campo {key} no existe en la entidad de Reviews.";

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idExistente)).Returns(review);
            _mockPropertyModelValidate.Setup(r => r.ValidarPropiedad<ReviewModel>(key)).Returns(false);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ModificarReviewPorId(idExistente, cambios));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Once);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Once);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Never);

        }

        [Theory]
        [InlineData("CALIFICACION", "fsdf")]
        [InlineData("COMENTARIO", 352)]
        public void ModificarReviewsPorId_ReseñaconCampoInvalido_ReturnsError(string key, object value)
        {
            //Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idExistente = review.Id;
            Dictionary<string, object> cambios = new Dictionary<string, object>()
            {
                { key, value }
            };
            string ErrorMessage = $"El campo {key} no es válido.";

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idExistente)).Returns(review);
            _mockPropertyModelValidate.Setup(r => r.ValidarPropiedad<ReviewModel>(key)).Returns(true);
            _mockReviewPartialUpdateValidations.Setup(r => r.Validate(key, value)).Returns(false);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseReview.ModificarReviewPorId(idExistente, cambios));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Once);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Once);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
        }

        [Theory]
        [InlineData("CALIFICACION", 3)]
        [InlineData("COMENTARIO", "Good Book")]
        public void ModificarReviewsPorId_NoActualizaReview_ReturnsError(string key, object value)
        {
            //Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idExistente = review.Id;
            Dictionary<string, object> cambios = new Dictionary<string, object>()
            {
                { key, value }
            };
            string ErrorMessage = "No se aplicaron cambios a la review.";

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idExistente)).Returns(review);
            _mockPropertyModelValidate.Setup(r => r.ValidarPropiedad<ReviewModel>(key)).Returns(true);
            _mockReviewPartialUpdateValidations.Setup(r => r.Validate(key, value)).Returns(true);
            _mockReviewRepositorio.Setup(r => r.UpdateReviewParcial(review,cambios)).Returns(false);

            //Act
            var exception = Assert.Throws<Exception>(() => _useCaseReview.ModificarReviewPorId(idExistente, cambios));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Once);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Once);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
        }


        [Theory]
        [InlineData("CALIFICACION", 3)]
        [InlineData("COMENTARIO", "Good Book")]
        public void ModificarReviewsPorId_ActualizaReview_ReturnsTrue(string key, object value)
        {
            //Arrange
            ReviewModel review = _reviewBuilderCaseTest.Build();
            long idExistente = review.Id;
            Dictionary<string, object> cambios = new Dictionary<string, object>()
            {
                { key, value }
            };

            _mockReviewRepositorio.Setup(r => r.ListReviewPorId(idExistente)).Returns(review);
            _mockPropertyModelValidate.Setup(r => r.ValidarPropiedad<ReviewModel>(key)).Returns(true);
            _mockReviewPartialUpdateValidations.Setup(r => r.Validate(key, value)).Returns(true);
            _mockReviewRepositorio.Setup(r => r.UpdateReviewParcial(review, cambios)).Returns(true);

            //Act
            bool Resultado = _useCaseReview.ModificarReviewPorId(idExistente, cambios);

            //Assert
            Assert.True(Resultado);
            _mockReviewRepositorio.Verify(r => r.ListReviewPorId(It.IsAny<long>()), Times.Once);
            _mockPropertyModelValidate.Verify(r => r.ValidarPropiedad<ReviewModel>(It.IsAny<string>()), Times.Once);
            _mockReviewPartialUpdateValidations.Verify(r => r.Validate(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            _mockReviewRepositorio.Verify(r => r.UpdateReviewParcial(It.IsAny<ReviewModel>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

    }
}
