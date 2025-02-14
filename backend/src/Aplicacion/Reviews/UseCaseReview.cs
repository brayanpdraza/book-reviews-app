using Aplicacion.Methods;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using Dominio.Servicios.ServicioValidaciones.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;

namespace Aplicacion.Reviews
{
    public class UseCaseReview
    {
        private readonly IReviewRepositorio _reviewRepositorio;
        private readonly ILibroRepositorio _libroRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IReviewValidations _reviewValidations;
        private readonly IReviewPartialUpdateValidations _reviewPartialUpdateValidations;
        //private readonly IpropertyModelValidate _propertyModelValidate;
        private readonly MetodosAuxiliares _metodosAuxiliares;

        public UseCaseReview(IReviewRepositorio reviewRepositorio,ILibroRepositorio libroRepositorio, IUsuarioRepositorio usuarioRepositorio, IReviewValidations reviewValidations,IReviewPartialUpdateValidations reviewPartialUpdateValidations,
            /*IpropertyModelValidate propertyModelValidate,*/MetodosAuxiliares metodosAuxiliares)
        {
            _reviewRepositorio = reviewRepositorio;
            _libroRepositorio = libroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _reviewValidations = reviewValidations;
            _reviewPartialUpdateValidations = reviewPartialUpdateValidations;
          //  _propertyModelValidate = propertyModelValidate;
            _metodosAuxiliares = metodosAuxiliares;
            
        }

        public long AddReview(ReviewModel review)
        {
            long idCreado;
            UsuarioModelo usuarioConsultado;
            LibroModelo libroConsultado;

            _reviewValidations.Validate(review);

            usuarioConsultado = _usuarioRepositorio.ListUsuarioPorId(review.Usuario.Id);
            if (usuarioConsultado.Id <= 0)
            {
                throw new KeyNotFoundException("El Usuario que intenta realizar la reseña no se encuentra registrado en el sistema.");
            }

            libroConsultado = _libroRepositorio.ListLibroPorId(review.Libro.Id);
            if (libroConsultado.Id <= 0)
            {
                throw new KeyNotFoundException("El libro al que intenta realizar la reseña no se encuentra registrado en el sistema.");
            }

            idCreado = _reviewRepositorio.AddReview(review);

            if (idCreado <= 0)
            {
                throw new Exception("La reseña no ha sido creada.");
            }

            return idCreado;
        }

        public ReviewModel ConsultarReviewsPoriD(long id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("No se puede consultar la reseña porque el id no es válido.");
            }

            return _reviewRepositorio.ListReviewPorId(id);

        }

        public PaginacionResultadoModelo<ReviewModel> ConsultarReviewsPorUsuarioPaginados(long usuarioid, int pagina, int tamanoPagina)
        {
            UsuarioModelo usuarioConsultado;
            List<ReviewModel> Reviews;
            int totalRegistros, skip, totalPaginas;

            if (usuarioid <= 0)
            {
                throw new ArgumentException("No se pueden consultar las reseñas porque el ID del usuario no es válido.");
            }
            if (pagina <= 0)
            {
                throw new ArgumentException("La página debe ser mayor a cero.");
            }

            if (tamanoPagina <= 0)
            {
                throw new ArgumentException("El tamaño de página debe ser mayor a cero.");
            }

            usuarioConsultado = _usuarioRepositorio.ListUsuarioPorId(usuarioid);
            if (usuarioConsultado.Id <= 0)
            {
                throw new KeyNotFoundException("El usuario al que intenta consultar sus reviews, no se encuentra en el sistema.");
            }

            totalRegistros = _reviewRepositorio.ConteoDistinctLibrosReviewsPorUsuario(usuarioConsultado);

            if (totalRegistros <= 0)
            {
                return new PaginacionResultadoModelo<ReviewModel>
                {
                    Items = new List<ReviewModel>(), // Lista vacía
                    TotalRegistros = 0,
                    PaginaActual = 1,
                    TamanoPagina = tamanoPagina
                };
            }

            totalPaginas = _metodosAuxiliares.TotalPaginas(totalRegistros, tamanoPagina);

            if (pagina > totalPaginas)
            {
                throw new ArgumentException($"La página solicitada ({pagina}) excede el total de páginas disponibles.");
            }

            skip = (pagina - 1) * tamanoPagina;

            Reviews = _reviewRepositorio.ListReviewPorUsuarioPaginado(usuarioConsultado,skip,tamanoPagina);


            return new PaginacionResultadoModelo<ReviewModel>
            {
                Items = Reviews,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };

        }

        public List<ReviewModel> ConsultarReviewsPorLibro(LibroModelo Libro)
        {
            LibroModelo libroConsultado;

            if (Libro == null)
            {
                throw new ArgumentException("No se pueden consultar reseñas porque el libro proporcionado es nulo.");
            }

            if (Libro.Id <= 0)
            {
                throw new ArgumentException("No se pueden consultar las reseñas porque el ID del libro no es válido.");
            }

            libroConsultado = _libroRepositorio.ListLibroPorId(Libro.Id);
            if (libroConsultado.Id <= 0)
            {
                throw new KeyNotFoundException("El libro al que intenta Consultar sus reseñas no se encuentra registrado en el sistema.");
            }

            return _reviewRepositorio.ListReviewPorLibro(Libro);

        }

        public bool ModificarReviewPorId(long id, Dictionary<string, object> cambios)
        {
            if (id <= 0)
            {
                throw new ArgumentException("No se puede modificar la reseña porque el ID no es válido.");
            }

            ReviewModel review = _reviewRepositorio.ListReviewPorId(id);
            if (review.Id <= 0)
            {
                throw new KeyNotFoundException("Review no encontrada.");
            }

            foreach (var cambio in cambios)
            {
                //if (!_propertyModelValidate.ValidarPropiedad<ReviewModel>(cambio.Key))
                //{
                //    throw new ArgumentException($"El campo {cambio.Key} no existe en la entidad de Reviews.");
                //}

                if (!_reviewPartialUpdateValidations.Validate(cambio.Key, cambio.Value))
                {
                    throw new ArgumentException($"El campo {cambio.Key} no es válido.");
                }
            
            }

            bool cambiosAplicados = _reviewRepositorio.UpdateReviewParcial(review, cambios);
            if (!cambiosAplicados)
            {
                throw new Exception("No se aplicaron cambios a la reseña.");
            }

            return true;
        }

        public bool EliminarReviewPorId(long id,long idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("No se puede eliminar la reseña porque el ID del Usuario no es válido.");
            }

            if (id <= 0)
            {
                throw new ArgumentException("No se puede eliminar la reseña porque el ID no es válido.");
            }

            UsuarioModelo usuario = _usuarioRepositorio.ListUsuarioPorId(idUsuario);

            if (usuario.Id <= 0)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado.");
            }

            ReviewModel review = _reviewRepositorio.ListReviewPorId(id);
            if (review.Id <= 0)
            {
                throw new KeyNotFoundException("Reseña no encontrada.");
            }

            bool ReviewEliminada = _reviewRepositorio.DeleteReview(review);
            if (!ReviewEliminada)
            {
                throw new Exception("No se eliminó la reseña.");
            }

            return true;
        }

    }
}
