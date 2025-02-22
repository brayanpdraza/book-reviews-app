using Microsoft.AspNetCore.Mvc.Testing;
using AdaptadorAPITest.Factories;
using AdaptadorAPITest.Contollers.UserController;
using System.Net;

namespace AdaptadorAPITest.UserController
{
    public class UserControllerITest : IClassFixture<CustomWebApplicationFactoryHelper>
    {
        private readonly HttpClient _client;
        private readonly string ControllerName = "Usuario";
        CustomWebApplicationFactory<Program> _customWebApplicationFactory;

        public UserControllerITest(CustomWebApplicationFactoryHelper helper)
        {
            _customWebApplicationFactory = helper.Factory;
            _client = _customWebApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task ObtenersuarioiD_IdInvalido_Retorna400()
        {
            // Arrange
            int idInvalido = 0;

            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerUsuarioid/{idInvalido}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ObtenersuarioiD_IdInexistente_Retorna404()
        {
            // Arrange
            int idInexistente = 2;


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerUsuarioid/{idInexistente}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(Skip = "No se requiere probar error 500 en este momento")]
        public async Task ObtenerUsuarioId_ErrorInterno_Retorna500()
        {
            int idQueGeneraError = -1;
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerUsuarioid/{idQueGeneraError}");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task ObtenersuarioiD_IdExistente_Retorna200()
        {
            // Arrange
            int idExistente = 1;

            // Act
            var response = await _client.GetAsync($"/{ControllerName}/ObtenerUsuarioid/{idExistente}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

    }
}