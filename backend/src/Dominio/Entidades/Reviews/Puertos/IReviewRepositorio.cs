using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Reviews.Puertos
{
    public interface IReviewRepositorio
    {
        List<ReviewModel> ListReviewPorLibro(LibroModelo Libro);
        //ReviewModel ListReviewLibroUsuario(LibroModelo Libro,UsuarioModelo Usuario);
        ReviewModel ListReviewPorId(long id);
        List<ReviewModel> ListReviewPorUsuarioPaginado(UsuarioModelo Usuario,int Skip, int TamanoPagina);
        int ConteoReviews(UsuarioModelo Usuario);
        long AddReview(ReviewModel Review);
        void SaveChanges();
    }
}
