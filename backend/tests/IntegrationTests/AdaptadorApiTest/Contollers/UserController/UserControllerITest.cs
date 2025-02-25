using Microsoft.AspNetCore.Mvc.Testing;
using AdaptadorAPITest.Factories;
using AdaptadorAPITest.Contollers.UserController;
using System.Net;
using Dominio.Entidades.Usuarios.Modelo;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Dominio.Usuarios.Modelo;

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


        [Theory(Skip ="Al ser enviados en la URL los parámetros, retorna 'NOTFOUND', por lo cual no es posible que los parámetros sean vacíos o nulos")]
        [InlineData(null, "")]
        [InlineData("","")]
        [InlineData("Correo", null)]
        [InlineData("Correo", "")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_ParamsInvalidos_Retorna400(string CorreoInvalido, string PassInvalida)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{CorreoInvalido}/{PassInvalida}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("test@example123.com", "P4ss123@")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_CorreoIncorrecto_Retorna404(string CorreoInexistente, string Pass)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{CorreoInexistente}/{Pass}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("test@example.com", "P4ss123@111")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_PassIncorrecto_Retorna401(string Correo, string PassIncorrecta)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{PassIncorrecta}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory(Skip = "No se requiere validar si el servicio de autenticacion genera mal los tokens")]
        [InlineData("test@example.com", "P4ss123@")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_TokenAutenticacionError_Retorna401(string Correo, string Pass)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Theory(Skip = "No se requiere probar error 500 en este momento")]
        [InlineData("test@example.com", "P4ss123@")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_ErrorInterno_Retorna500(string Correo, string Pass)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("test@example.com", "P4ss123@")]
        public async Task AutenticacionUsuarioPorCorreoYPassword_DatosCorrectos_Retorna200(string Correo, string Pass)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //********************************************************

        [Theory]
        [InlineData("",HttpStatusCode.Unauthorized)]
        [InlineData(null,HttpStatusCode.BadRequest)]
        public async Task UpdateRefreshTokenBySelf_TokenNullOrEmpty_Retorna401400(string RefreshToken,HttpStatusCode Code)
        {
            // Arrange


            var contenido = new StringContent(JsonConvert.SerializeObject(RefreshToken), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/update-refresh-token", contenido);

            // Assert
            Assert.Equal(Code, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Theory]
        [InlineData("fasggsghshshshshshshshsdhshs")]
        [InlineData("gGsdgsdg454GSGDS4G56gfgdfhdf")]
        [InlineData("jhgj")]
        public async Task UpdateRefreshTokenBySelf_BadToken_Retorna401(string RefreshToken)
        {
            // Arrange


            var contenido = new StringContent(JsonConvert.SerializeObject(RefreshToken), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/update-refresh-token", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Theory]
        [InlineData("TokenPruebaExpírado")]
        public async Task UpdateRefreshTokenBySelf_RefreshTokenVencido_Retorna401(string RefreshToken)
        {
            // Arrange

            var contenido = new StringContent(JsonConvert.SerializeObject(RefreshToken), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/update-refresh-token", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Theory(Skip = "No se requiere probar error 500 en este momento")]
        [InlineData("test@example.com", "P4ss123@")]
        public async Task UpdateRefreshTokenBySelf_ErrorInterno_Retorna500(string Correo, string Pass)
        {
            // Arrange
            var loginResponse = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var authResult = JsonConvert.DeserializeObject<AuthenticationResult>(loginContent);
            var contenido = new StringContent(JsonConvert.SerializeObject(authResult.RenewalCredential), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/update-refresh-token", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Theory]
        [InlineData("test@example.com", "P4ss123@")]
        public async Task UpdateRefreshTokenBySelf_TokenCorrecto_Retorna200(string Correo, string Pass)
        {
            // Arrange

            var loginResponse = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            var authResult = JsonConvert.DeserializeObject<AuthenticationResult>(loginContent);
            var contenido = new StringContent(JsonConvert.SerializeObject(authResult.RenewalCredential), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/update-refresh-token", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        //*********************************************

        [Fact]
        public async Task GuardarUsuario_ModelInvalido_Retorna404()
        {
            // Arrange

            var usuarioInvalido = new
            {
                nombre = "Soy un modelo Invalido",
                correo = "correo@malo.com"
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioInvalido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Fact]
        public async Task GuardarUsuario_UsuarioNull_Retorna409()
        {
            // Arrange

            UsuarioModelo usuarioInvalido = null;

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioInvalido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Theory]
        [InlineData("Correomalo","passbad")]
        [InlineData("Correobueno@gmail.com", "passbad")]
        public async Task GuardarUsuario_UsuarioInvalido_Retorna400(string CorreoInvalido, string PassInvalida)
        {
            // Arrange

            UsuarioModelo usuarioInvalido = new UsuarioModelo
            {
                Nombre = "Usuario Malo",
                Correo = CorreoInvalido,
                Password = PassInvalida
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioInvalido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Fact]
        public async Task GuardarUsuario_UsuarioCorreoYaRegistrado_Retorna409()
        {
            // Arrange

            UsuarioModelo usuarioInvalido = new UsuarioModelo
            {
                Nombre = "Estoy Repetido",
                Correo = "test@example.com",
                Password = "P4ss123@",         
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioInvalido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Fact(Skip ="No se requiere validar error 500")]
        public async Task GuardarUsuario_ErrorGenericoUsuarioNoCreado_Retorna500()
        {
            // Arrange

            UsuarioModelo usuarioInvalido = new UsuarioModelo
            {
                Nombre = "Estoy Repetido",
                Correo = "test@example.com",
                Password = "P4ss123@",
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioInvalido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        [Fact]
        public async Task GuardarUsuario_UsuarioCreado_Retorna200()
        {
            // Arrange

            UsuarioModelo usuarioValido = new UsuarioModelo
            { 
                Nombre = "Nombre Correcto",
                Correo = "Correo@correcto.com",
                Password = "P4ss123@",           
            };

            var contenido = new StringContent(JsonConvert.SerializeObject(usuarioValido), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/{ControllerName}/GuardarUsuario", contenido);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }
    }
}