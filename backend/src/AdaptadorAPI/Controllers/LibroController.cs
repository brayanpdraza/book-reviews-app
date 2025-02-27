using Aplicacion.Libros;
using Dominio.Libros.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LibroController : Controller
    {
        private readonly UseCaseLibro _useCaseLibro;
        private readonly ILogger<LibroController> _logger;

        public LibroController(UseCaseLibro useCaseLibro, ILogger<LibroController> logger)
        {
            _useCaseLibro = useCaseLibro;
            _logger = logger;
        }

        [HttpGet("ObtenerLibroPorid/{id}", Name = "ObtenerLibroPorid")]
        public IActionResult ObtenerLibroPorid(long id)
        {
            LibroModelo Libro;
            try
            {
                Libro = _useCaseLibro.ConsultarLibroPorId(id);
                return Ok(Libro);

            }
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener libro con id: {id}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Libro con id: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener Libro con ID: {id}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }


        }

        [HttpGet("ConsultarLibrosPaginadosFiltroOpcional/{pagina}/{tamanoPagina}")]
        public IActionResult ConsultarLibrosPaginadosFiltroOpcional(int pagina, int tamanoPagina, [FromQuery] string? filtro = null)
        {
            PaginacionResultadoModelo<LibroModelo> libros;
            try
            {
                libros = _useCaseLibro.ConsultarLibrosPaginados(pagina, tamanoPagina, filtro);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message, $"Al obtener libro paginado. Filtro: {filtro}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener libro con Filtro: {filtro}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            return Ok(libros);
        }
    }
}
