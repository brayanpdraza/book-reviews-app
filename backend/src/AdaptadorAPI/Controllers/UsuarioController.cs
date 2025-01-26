using Aplicacion.Usuarios;
using Dominio.Usuarios.Modelo;
using Microsoft.AspNetCore.Mvc;
using Dominio.Usuarios.Puertos;
using Dominio.Entidades.Usuarios.Modelo;
using Microsoft.IdentityModel.Tokens;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UseCaseUsuario _useCaseUsuario;
        private readonly IAuthService _authServie;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(UseCaseUsuario usuarioCaseUsuario, IAuthService authServie, ILogger<UsuarioController> logger)
        {
            _useCaseUsuario = usuarioCaseUsuario;
            _authServie = authServie;
            _logger = logger;
        }

        [HttpGet("ObtenerUsuarioid/{id}", Name = "ObtenerUsuarioid")]
        public IActionResult ObtenerUsuarioid(long id)
        {
            UsuarioModelo usuario;
            try
            {
                usuario = _useCaseUsuario.ConsultarUsuarioPorId(id);
            }
            catch (KeyNotFoundException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message,$"Al obtener usuario con id: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener usuario con ID: {id}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            return Ok(usuario);

        }

        [HttpGet("AutenticacionUsuarioPorCorreoYPasssword/{Correo}/{Password}")]
        public IActionResult AutenticacionUsuarioPorCorreoYPasssword(string Correo, string Password)
        {
            AuthenticationResult responseLogin;
            try
            {
                responseLogin = _useCaseUsuario.AutenticacionByCredenciales(Correo,Password);
            }
            catch (KeyNotFoundException ex) 
            {
                _logger.LogWarning(ex.Message,$"Al Consultar un usuario con credenciales: {Correo}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener usuario con credenciales: {Correo}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            return Ok(responseLogin);

        }

        [HttpPost("update-refresh-token")]
        public IActionResult UpdateRefreshTokenBySelf([FromBody] string refreshToken)
        {
            AuthenticationResult responseLogin;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                responseLogin = _useCaseUsuario.UpdateRefreshToken(refreshToken);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message, $"Al Consultar un usuario con credenciales: {refreshToken}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al Actualizar Token: {refreshToken}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            return Ok(responseLogin);

        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _useCaseUsuario.LogOutByAccessToken(request.Credential); //Access Token
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "El token ha expirado.");
                return Unauthorized("El token ha expirado.");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido.");
                return Unauthorized("Token inválido.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión.");
                return StatusCode(500, "Error interno.");
            }

            return NoContent();

        }

        [HttpPost("GuardarUsuario")]
        public IActionResult GuardarUsuario([FromBody] UsuarioModelo usuario)
        {
            string uri;
            long createdId = 0;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                createdId = _useCaseUsuario.AddUsuario(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al guardar usuario: {usuario}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            if (createdId == 0)
            {
                return BadRequest("No se creó el usuario. Por favor valide");
            }

            uri = Url.Link("ObtenerUsuarioId", new { id = createdId });

            if (uri == null)
            {
                return BadRequest("No se pudo generar la URI para acceder al nuevo usuario.");
            }

            return Created(uri, usuario);
        }


    }
}
