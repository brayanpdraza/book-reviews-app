using Aplicacion.Libros;
using Aplicacion.Methods;
using AplicacionTest.Libros;
using Dominio.Entidades.Libros.Puertos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionTest.Methods
{
    public class MetodosAuxiliaresTest
    {
        private readonly MetodosAuxiliares _metodosAuxiliares;

        public MetodosAuxiliaresTest()
        {
            _metodosAuxiliares = new MetodosAuxiliares();
        }


        [Theory]
        [InlineData(0, 10, 0)]    // No hay registros
        [InlineData(10, 10, 1)]   // Exactamente una página
        [InlineData(15, 10, 2)]   // Página extra por registros no exactos
        [InlineData(1, 10, 1)]    // Un solo registro
        [InlineData(1000, 100, 10)] // Muchas páginas
        public void TotalPaginas_CalculaCorrectamente(int totalRegistros, int tamanoPagina, int esperado)
        {
            // Act
            var resultado = _metodosAuxiliares.TotalPaginas(totalRegistros, tamanoPagina);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void TotalPaginas_TamanoPaginaCero_LanzaExcepcion()
        {
            // Arrange
            int totalRegistros = 10;
            int tamanoPagina = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _metodosAuxiliares.TotalPaginas(totalRegistros, tamanoPagina));
        }
    }
}
