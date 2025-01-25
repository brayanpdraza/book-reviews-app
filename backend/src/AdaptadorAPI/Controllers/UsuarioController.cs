using AdaptadorAPI.Contratos;
using AdaptadorAPI.Implementaciones;
using AdaptadorPostgreSQL.Usuarios.Adaptadores;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Aplicacion.Usuarios;
using Dominio.Usuarios.Modelo;
using Microsoft.AspNetCore.Mvc;
using Dominio.Usuarios.Puertos;
using Dominio.Entidades.Usuarios.Modelo;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UseCaseUsuario _useCaseUsuario;
        private readonly IAuthService _authServie;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(UseCaseUsuario usuarioRepositorio, IAuthService authServie, ILogger<UsuarioController> logger)
        {
            _useCaseUsuario = usuarioRepositorio;
            _authServie = authServie;
            _logger = logger;
        }

        [HttpGet("ObtenerUsuarioCredenciales/{id}", Name = "ObtenerUsuarioId")]
        public IActionResult ObtenerUsuarioid(long id)
        {
            UsuarioModelo usuario;
            try
            {
                usuario = _useCaseUsuario.ConsultarUsuarioPorId(id);
                return Ok(usuario);
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
        }

        [HttpGet("ConsultarUsuarioPorCredenciales/{Correo}/{Password}")]
        public IActionResult ConsultarUsuarioPorCredenciales(string Correo, string Password)
        {
            AuthenticationResult responseLogin;
            try
            {
                responseLogin = _useCaseUsuario.ConsultarUsuarioCredenciales(Correo,Password);

                return Ok(responseLogin);
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
