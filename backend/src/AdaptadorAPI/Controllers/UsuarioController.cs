using Aplicacion.Usuarios;
using Dominio.Usuarios.Modelo;
using Microsoft.AspNetCore.Mvc;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UseCaseUsuario _useCaseUsuario;

        public UsuarioController(UseCaseUsuario usuarioRepositorio)
        {
            _useCaseUsuario = usuarioRepositorio;
        }

        [HttpGet("ObteerUsuarioCredenciales/{id}", Name = "ObtenerUsuarioId")]
        public IActionResult ObteerUsuarioid(string Correo, string Password)
        {
            //UsuarioModelo usuario;
            //try
            //{
            //    usuario = _useCaseUsuario.ConsultarUsuarioCredenciales(Correo, Password);
            //    return Ok(usuario);
            //}
            //catch (FileNotFoundException ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    return NotFound(ex.Message);
            //}
            throw new NotImplementedException();
        }

        [HttpGet("ObtenerUsuarioCredenciales/{Correo}/{Password}", Name = "ObtenerUsuarioCredenciales")]
        public IActionResult ObtenerUsuarioCredenciales(string Correo, string Password)
        {
            UsuarioModelo usuario;
            try
            {
                usuario = _useCaseUsuario.ConsultarUsuarioCredenciales(Correo,Password);
                return Ok(usuario);
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("GuardarUsuario")]
        public IActionResult GuardarUsuario([FromBody] UsuarioModelo usuario)
        {
            string uri;
            long createdId = 0;

            try
            {
                createdId = _useCaseUsuario.AddUsuario(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
