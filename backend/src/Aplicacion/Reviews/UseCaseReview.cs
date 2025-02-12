﻿using Aplicacion.Methods;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
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
        private readonly MetodosAuxiliares _metodosAuxiliares;

        public UseCaseReview(IReviewRepositorio reviewRepositorio,ILibroRepositorio libroRepositorio, IUsuarioRepositorio usuarioRepositorio, IReviewValidations reviewValidations, MetodosAuxiliares metodosAuxiliares)
        {
            _reviewRepositorio = reviewRepositorio;
            _libroRepositorio = libroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _reviewValidations = reviewValidations;
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
        //REALIZAR PRUEBAS UNITARIAS
        public PaginacionResultadoModelo<ReviewModel> ConsultarReviewsPorUsuarioPaginados(UsuarioModelo Usuario, int pagina, int tamanoPagina)
        {
            UsuarioModelo usuarioConsultado;
            List<ReviewModel> Reviews;
            int totalRegistros, skip, totalPaginas;

            if (Usuario == null)
            {
                throw new ArgumentException("No se pueden consultar reseñas porque el usuario proporcionado es nulo.");
            }

            if (Usuario.Id <= 0)
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

            usuarioConsultado = _usuarioRepositorio.ListUsuarioPorId(Usuario.Id);
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

    }
}
