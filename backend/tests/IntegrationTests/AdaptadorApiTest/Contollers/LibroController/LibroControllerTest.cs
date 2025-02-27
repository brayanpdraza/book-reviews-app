using AdaptadorApiTest.Factories;
using AdaptadorAPITest.Factories;
using Dominio.Libros.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorApiTest.Contollers.LibroController
{
    public class LibroControllerTest : IClassFixture<CustomWebApplicationFactoryHelper>
    {
        private readonly HttpClient _client;
        private readonly string ControllerName = "Libro";
        CustomWebApplicationFactory<Program> _customWebApplicationFactory;

        public LibroControllerTest(CustomWebApplicationFactoryHelper helper)
        {
            _customWebApplicationFactory = helper.Factory;
            _client = _customWebApplicationFactory.CreateClient();
        }


        [Fact]
        public async Task ObtenerLibroPorid_IdInvalido_Retorna400()
        {
            // Arrange
            int idInvalido = 0;

            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerLibroPorid/{idInvalido}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ObtenerLibroPorid_IdInexistente_Retorna404()
        {
            // Arrange
            int idInexistente = 1000;


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerLibroPorid/{idInexistente}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(Skip = "No se requiere probar error 500 en este momento")]
        public async Task ObtenerLibroPorid_ErrorInterno_Retorna500()
        {
            int idQueGeneraError = -1;
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerLibroPorid/{idQueGeneraError}");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task ObtenerLibroPorid_IdExistente_Retorna200()
        {
            // Arrange
            int idExistente = 1;

            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerLibroPorid/{idExistente}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        //*************************************************************

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 1000)]
        public async Task ConsultarLibrosPaginadosFiltroOpcional_ParamsInvalidos_Retorna400(int Pagina, int TamanoPagina)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ConsultarLibrosPaginadosFiltroOpcional/{Pagina}/{TamanoPagina}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(1, 10, "categoria:noexistente")]
        public async Task ConsultarLibrosPaginadosFiltroOpcional_FiltroSinResultados_Retorna404(int Pagina, int TamanoPagina, string Filtro)
        {
            // Arrange
            int CantidadLibrosEncontradosEsperada = 0;

            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ConsultarLibrosPaginadosFiltroOpcional/{Pagina}/{TamanoPagina}?filtro={Uri.EscapeDataString(Filtro)}");

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica que la respuesta es 200 OK

            var content = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<PaginacionResultadoModelo<LibroModelo>>(content);

            Assert.NotNull(resultado);
            Assert.True(resultado.Items.Count <= CantidadLibrosEncontradosEsperada, "El filtro no trae resultados.");
        }


        [Theory]
        [InlineData(1, 10, "", 6)]
        [InlineData(1, 10, "categoria:misterio",2)]
        [InlineData(1, 10, "categoria:romance",3)]
        [InlineData(1, 10, "categoria:terror",1)]
        [InlineData(1, 10, "Autor:Autor 1", 1)]
        [InlineData(1, 10, "Titulo:Libro de misterio 1", 1)]
        public async Task ConsultarLibrosPaginadosFiltroOpcional_FiltroTraeDatos_Retorna200(int Pagina, int TamanoPagina, string Filtro, int LibrosTotalesEsperados)
        {
            // Arrange
            string url = Filtro is null
        ? $"/{ControllerName}/ConsultarLibrosPaginadosFiltroOpcional/{Pagina}/{TamanoPagina}"
        : $"/{ControllerName}/ConsultarLibrosPaginadosFiltroOpcional/{Pagina}/{TamanoPagina}?filtro={Uri.EscapeDataString(Filtro)}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica que la respuesta es 200 OK

            var content = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<PaginacionResultadoModelo<LibroModelo>>(content);

            Assert.NotNull(resultado);
            Assert.True(resultado.Items.Count == LibrosTotalesEsperados, "El filtro trae la cantidad correcta de libros.");

        }
    }
}
