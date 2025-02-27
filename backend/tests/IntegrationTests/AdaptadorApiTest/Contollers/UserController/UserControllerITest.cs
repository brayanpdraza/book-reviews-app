using Microsoft.AspNetCore.Mvc.Testing;
using AdaptadorAPITest.Factories;
using System.Net;
using Dominio.Entidades.Usuarios.Modelo;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Dominio.Usuarios.Modelo;
using System.Text.Json;
using AdaptadorApiTest.Factories;

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

        //************************************************************************

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
        [InlineData("test@example.com", "P4ss123@")]
        [InlineData("test@example.com", "P4ss123@")]
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

        //*************************************************************************************************
        [Theory]
        [InlineData(null, HttpStatusCode.Unauthorized)]  // No enviar header
        [InlineData("Bearer invalid_token", HttpStatusCode.Unauthorized)]  // Token inválido
        public async Task Logout_InvalidToken_Retorna401(string token, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, $"/{ControllerName}/logout");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact(Skip = "No es viable ni necesario validar, puesto que son relacionados al token, y el sistema está hecho para que el token tenga los campos correctos")]
        public async Task Logout_ArgumentExcepction_Retorna400()
        { }

        [Fact(Skip = "No es viable ni necesario validar, puesto que son relacionados al token, y el sistema está hecho para que el token tenga los campos correctos")]
        public async Task Logout_SecurityTokenException_Retorna401()
        { }
        [Fact(Skip = "No es viable ni necesario validar. Solo sucede si el usuario no está registrado. Es un caso aislado donde se elimina el usuario mientras hay una sesión activa (lapso de 15 minutos), se puede realizar haciendo un inicio de sesion luego se elimina el usuario y luego se hace el logout. No hay método de delete de usuarios implementado")]
        public async Task Logout_UnauthorizedAccessException_Retorna401()
        { }
        [Fact(Skip = "No se requiere validar las excepciones 500")]
        public async Task Logout_OtherExceptions_Retorna500()
        { }

        [Theory]
        [InlineData("test@example.com", "P4ss123@", HttpStatusCode.NoContent)] 
        public async Task Logout_TokenValido_Retorna200(string Correo, string Pass, HttpStatusCode expectedStatusCode)
        {
            // Arrange

            var loginResponse = await _client.GetAsync($"/{ControllerName}/AutenticacionUsuarioPorCorreoYPassword/{Correo}/{Pass}");

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode); // Verificamos que el login fue exitoso

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(loginContent);
            var token = jsonResponse.RootElement.GetProperty("credential").GetString(); // Extraemos el token

            Assert.False(string.IsNullOrEmpty(token), "El token no debe ser nulo o vacío.");

            var request = new HttpRequestMessage(HttpMethod.Post, $"/{ControllerName}/logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}