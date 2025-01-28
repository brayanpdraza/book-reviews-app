using Aplicacion.Reviews;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaptadorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly UseCaseReview _useCaseReview;
        private readonly ILogger<LibroController> _logger;

        public ReviewController(UseCaseReview useCaseReview, ILogger<LibroController> logger)
        {
            _useCaseReview = useCaseReview;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("GuardarReview")]
        public IActionResult GuardarReview([FromBody] ReviewModel review)
        {
            string uri;
            long createdId ;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                createdId = _useCaseReview.AddReview(review);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message, $"Al Guardar Review: {review}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al guardar usuario: {review}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }

            if (createdId == 0)
            {
                return BadRequest("No se creó la reseña. Por favor valide");
            }

            uri = Url.Link("ObtenerReviewPorId", new { id = createdId });

            if (uri == null)
            {
                return BadRequest("No se pudo generar la URI para acceder a la nueva reseña.");
            }

            return Created(uri, review);
        }

        [HttpGet("ObtenerReviewPorId/{id}", Name = "ObtenerReviewPorId")]
        public IActionResult ObtenerReviewPorId(long id)
        {
            ReviewModel Review;
            try
            {
                Review = _useCaseReview.ConsultarReviewsPoriD(id);
                return Ok(Review);

            }
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Review con id: {id}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Review con id: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener Review con ID: {id}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }
        }

        [HttpPost("ConsultarReviewsPorLibro")]
        public IActionResult ConsultarReviewsPorLibro([FromBody] LibroModelo request)
        {
            List<ReviewModel> Reviews;
            try
            {
                Reviews = _useCaseReview.ConsultarReviewsPorLibro(request);
                return Ok(Reviews);

            }
            catch (ArgumentException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Reseñas con libro: {request}");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)  // Más apropiado para "no encontrado"
            {
                _logger.LogWarning(ex.Message, $"Al obtener Reseñas con libro: {request}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error interno al obtener Reseñas para el libro: {request}");
                return StatusCode(500, $"Ocurrió un error interno. Por favor, contacta al soporte. {ex.Message}");
            }
        }
    }
}
