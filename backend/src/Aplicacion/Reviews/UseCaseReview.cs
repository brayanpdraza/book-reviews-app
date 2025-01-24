using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Dominio.Usuarios.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Reviews
{
    public class UseCaseReview
    {
        private readonly IReviewRepositorio _reviewRepositorio;
        private readonly ILibroRepositorio _libroRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IReviewValidations _reviewValidations;

        public UseCaseReview(IReviewRepositorio reviewRepositorio,ILibroRepositorio libroRepositorio, IUsuarioRepositorio usuarioRepositorio, IReviewValidations reviewValidations)
        {
            _reviewRepositorio = reviewRepositorio;
            _libroRepositorio = libroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _reviewValidations = reviewValidations;
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
                throw new ArgumentException("El Usuario que intenta realizar la reseña no se encuentra registrado en el sistema.");
            }

            libroConsultado = _libroRepositorio.ListLibroPorId(review.Libro.Id);
            if (libroConsultado.Id <= 0)
            {
                throw new ArgumentException("El libro al que intenta realizar la reseña no se encuentra registrado en el sistema.");
            }

            idCreado = _reviewRepositorio.AddReview(review);

            if (idCreado <= 0)
            {
                throw new Exception("La reseña no ha sido creada.");
            }

            return idCreado;
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
                throw new ArgumentException("El libro al que intenta Consultar sus reseñas no se encuentra registrado en el sistema.");
            }

            return _reviewRepositorio.ListReviewPorLibro(Libro);

        }

    }
}
