using Aplicacion.Libros;
using Aplicacion.Usuarios;
using AplicacionTest.Usuarios;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using Dominio.Usuarios.Puertos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionTest.Libros
{
    public class LibrosUseCaseTest
    {
        private readonly Mock<ILibroRepositorio> _mockLibroRepositorio;

        private readonly UseCaseLibro _useLibro;
        private readonly LibrosBuilderUseCaseTest _librosBuilderCaseTest;

        public LibrosUseCaseTest()
        {
            _mockLibroRepositorio = new Mock<ILibroRepositorio>();

            _useLibro = new UseCaseLibro(
                _mockLibroRepositorio.Object
            );
            _librosBuilderCaseTest = new LibrosBuilderUseCaseTest();
        }

        [Theory]
        [InlineData(0,1, "La página debe ser mayor a cero.")]
        [InlineData(1, 0, "El tamaño de página debe ser mayor a cero.")]
        public void ConsultarLibrosPaginados_Validacionpaginas_LanzaExcepcion(int Pagina, int tamanoPagina, string MessageError)
        {
            // Arrange


            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useLibro.ConsultarLibrosPaginados(Pagina, tamanoPagina));

            // Assert
            Assert.Equal(MessageError, exception.Message);


            _mockLibroRepositorio.Verify(r => r.ConteoLibros(""), Times.Never);
            _mockLibroRepositorio.Verify(r => r.ListLibrosPaginadosPorFiltroOpcional(Pagina,tamanoPagina,""), Times.Never);
        }

        [Theory]
        [InlineData(2, 1,0,"")]
        public void ConsultarLibrosPaginados_ConteoLibros0_ReturnsEmptyLibros(int Pagina, int tamanoPagina, int TotalLibros,string Filtro)
        {
            // Arrange
            List<LibroModelo> libroVacio = new List<LibroModelo>();
            int PaginaResult = 1;

            _mockLibroRepositorio.Setup(r=>r.ConteoLibros(Filtro)).Returns(TotalLibros);

            //Act
            PaginacionResultadoModelo<LibroModelo> Resultado = _useLibro.ConsultarLibrosPaginados(Pagina, tamanoPagina, Filtro);

            // Assert
            Assert.Equal(libroVacio, Resultado.Items);
            Assert.Equal(PaginaResult, Resultado.PaginaActual);
            Assert.Equal(tamanoPagina, Resultado.TamanoPagina);

            _mockLibroRepositorio.Verify(r => r.ConteoLibros(Filtro), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibrosPaginadosPorFiltroOpcional(Pagina, tamanoPagina, Filtro), Times.Never);
        }

        [Theory]
        [InlineData(4, 10, 29,"")]
        public void ConsultarLibrosPaginados_PaginaExcedeTotalPaginas_LanzaExcepcion(int Pagina, int tamanoPagina, int TotalLibros, string Filtro)
        {
            List<LibroModelo> libroVacio = new List<LibroModelo>();
            string MessageError = $"La página solicitada ({Pagina}) excede el total de páginas disponibles.";

            _mockLibroRepositorio.Setup(r => r.ConteoLibros(Filtro)).Returns(TotalLibros);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useLibro.ConsultarLibrosPaginados(Pagina, tamanoPagina, Filtro));

            // Assert
            Assert.Equal(MessageError, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ConteoLibros(It.IsAny<string>()), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibrosPaginadosPorFiltroOpcional(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(3, 10, 50,"")]
        public void ConsultarLibrosPaginados_ValidData_ReturnsLibros(int Pagina, int tamanoPagina, int TotalLibros,string Filtro)
        {
            // Arrange
            List<LibroModelo> libro = new List<LibroModelo>{ _librosBuilderCaseTest.Build() };

            _mockLibroRepositorio.Setup(r => r.ConteoLibros("")).Returns(TotalLibros);
            _mockLibroRepositorio.Setup(r => r.ListLibrosPaginadosPorFiltroOpcional(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(libro);

            //Act
            PaginacionResultadoModelo<LibroModelo> Resultado = _useLibro.ConsultarLibrosPaginados(Pagina, tamanoPagina, Filtro);

            // Assert
            Assert.Equal(libro, Resultado.Items);
            Assert.Equal(Pagina, Resultado.PaginaActual);
            Assert.Equal(tamanoPagina, Resultado.TamanoPagina);

            _mockLibroRepositorio.Verify(r => r.ConteoLibros(It.IsAny<string>()), Times.Once);
            _mockLibroRepositorio.Verify(r => r.ListLibrosPaginadosPorFiltroOpcional(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(0, "Debe ingresar un ID Válido para consultar un libro.")]
        public void ConsultarLibroPorId_idMenorQueCero_LanzaExcepcion(long id, string ErrorMessagge)
        {
            // Arrange

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useLibro.ConsultarLibroPorId(id));

            // Assert
            Assert.Equal(ErrorMessagge, exception.Message);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(It.IsAny<long>()), Times.Never);
        }

        [Theory]
        [InlineData(1)]
        public void ConsultarLibroPorId_idValido_ReturnsLibro(long id)
        {
            // Arrange
            LibroModelo LibroResult;
            LibroModelo LibroConsultado = _librosBuilderCaseTest.Build();

            _mockLibroRepositorio.Setup(r => r.ListLibroPorId(id)).Returns(LibroConsultado);
            //Act
            LibroResult = _useLibro.ConsultarLibroPorId(id);

            // Assert
            Assert.Equal(LibroConsultado, LibroResult);

            _mockLibroRepositorio.Verify(r => r.ListLibroPorId(It.IsAny<long>()), Times.Once);
        }
    }
}
