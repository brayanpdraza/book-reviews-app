using Aplicacion.Usuarios;
using Dominio.Usuarios.Modelo;
using Microsoft.AspNetCore.Mvc;
using Dominio.Usuarios.Puertos;
using Dominio.Entidades.Usuarios.Modelo;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using AdaptadorAPI.Servicios;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UseCaseUsuario _useCaseUsuario;
        private readonly IAuthService _authServie;
        private readonly ILogger<UsuarioController> _logger;
        private readonly ObtenerDatosUsuarioToken _obteneDatosUsuarioToken;

        public UsuarioController(UseCaseUsuario usuarioCaseUsuario, IAuthService authServie, ILogger<UsuarioController> logger,ObtenerDatosUsuarioToken obtenerDatosUsuarioToken)
        {
            _useCaseUsuario = usuarioCaseUsuario;
            _authServie = authServie;
            _logger = logger;
            _obteneDatosUsuarioToken = obtenerDatosUsuarioToken;
        }

        [HttpGet("ObtenerUsuarioid/{id}", Name = "ObtenerUsuarioid")]
        public IActionResult ObtenerUsuarioid(long id)
        {
            UsuarioModelo usuario;
            try
            {
                usuario = _useCaseUsuario.ConsultarUsuarioPorId(id);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message, $"Al obtener Usuario con id: {id}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex) 
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

        [HttpGet("AutenticacionUsuarioPorCorreoYPassword/{correo}/{password}")]
        public IActionResult AutenticacionUsuarioPorCorreoYPassword(string correo,string password)
        {
            AuthenticationResult responseLogin;
            try
            {
                responseLogin = _useCaseUsuario.AutenticacionByCredenciales(correo, password);
            }
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Usuario con Credenciales: {correo}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex) 
            {
                _logger.LogWarning(ex.Message,$"Al Consultar un usuario con credenciales: {correo}");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener usuario con credenciales: {correo}");
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
                return Ok(responseLogin);

            }

            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message, $"Al Actualizar token de refresco: {refreshToken}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al Actualizar Token: {refreshToken}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

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
            catch (DataException ex) 
            {
                _logger.LogWarning(ex.Message, $"Al Guardar usuario: {usuario.Correo}");
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message, $"Al Guardar usuario: {usuario.Correo}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al guardar usuario: {usuario.Correo}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            uri = Url.Link("ObtenerUsuarioId", new { id = createdId });

            if (uri == null)
            {
                return BadRequest("No se pudo generar la URI para acceder al nuevo usuario.");
            }

            return Created(uri, usuario);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            long idUsuario;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                idUsuario = _obteneDatosUsuarioToken.ObtenerIdUsuario(HttpContext);
                _useCaseUsuario.LogOutById(idUsuario); //Access Token
                return NoContent();

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Error al cerrar la sesión");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message, $"Token Incorrecto");
                return BadRequest(ex.Message);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido.");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión.");
                return StatusCode(500, "Error interno.");
            }

        }

    }
}
