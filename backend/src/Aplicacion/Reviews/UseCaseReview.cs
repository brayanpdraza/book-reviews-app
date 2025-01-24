using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Servicios.ServicioEncripcion.Contratos;
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
        private readonly IReviewValidations _reviewValidations;

        public UseCaseReview(IReviewRepositorio reviewRepositorio, IReviewValidations reviewValidations)
        {
            _reviewRepositorio = reviewRepositorio;
            _reviewValidations = reviewValidations;
        }

        public long AddReview(ReviewModel review)
        {
            long idCreado;
            //ReviewModel reviewExistente;

            _reviewValidations.Validate(review);

            //reviewExistente = _reviewRepositorio.ListReviewLibroUsuario(review.Libro, review.Usuario);

            //if (reviewExistente != null && reviewExistente.Id > 0)
            //{
            //    throw new Exception("No puede ingresar más de una reseña a un mismo libro.");
            //}

            idCreado = _reviewRepositorio.AddReview(review);

            if (idCreado <= 0)
            {
                throw new Exception("La reseña no ha sido creada");
            }

            return idCreado;
        }

        public List<ReviewModel> ConsultarReviewsPorLibro(LibroModelo Libro)
        {

            if (Libro == null)
            {
                throw new ArgumentException("No se pueden consultar reseñas porque el libro proporcionado es nulo.");
            }

            if (Libro.Id <= 0)
            {
                throw new ArgumentException("No se pueden consultar las reseñas porque el ID del libro no es válido");
            }

            return _reviewRepositorio.ListReviewPorLibro(Libro);

        }

    }
}
