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
        List<ReviewModel> ListReviewLibro(LibroModelo Libro);
        ReviewModel ListReviewPorId(long id);
        long AddReview(ReviewModel Review);
        void SaveChanges();
    }
}
