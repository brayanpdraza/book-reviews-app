using Aplicacion.Usuarios;
using Dominio.Usuarios.Modelo;
using Microsoft.AspNetCore.Mvc;
using Dominio.Usuarios.Puertos;
using Dominio.Entidades.Usuarios.Modelo;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Usuario con id: {id}");
                return BadRequest(ex.Message);
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

        [HttpGet("AutenticacionUsuarioPorCorreoYPassword/{correo}/{password}")] // Sin typo
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
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al actualizar token de refresco: {refreshToken}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message, $"Al Actualizar token de refresco: {refreshToken}");
                return NotFound(ex.Message);
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

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            int idUsuario;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // Obtener el usuario actual (ClaimsPrincipal)
                var usuario = HttpContext.User;

                // Extraer el claim "id" (usuarioId)
                var usuarioId = usuario.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(usuarioId))
                {
                    return Unauthorized("Claim 'id' no encontrado en el token.");
                }


                if (!long.TryParse(usuarioId, out long userId))
                {
                    // Usa el ID convertido (userId)
                    return BadRequest("No se pudo obtener un ID válido del token.");
                }


                _useCaseUsuario.LogOutById(userId); //Access Token
                return NoContent();

            }
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Token Incorrecto");
                return BadRequest(ex.Message);
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
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al Guardar usuario: {usuario.Correo}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al guardar usuario: {usuario.Correo}");
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
