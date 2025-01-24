using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Libros.Puertos
{
    internal interface ILibroRepositorio
    {
        List<LibroModelo> ListLibroAll();
        List<LibroModelo> ListLibroPorCategoria(string Categoria);
        List<LibroModelo> ListLibroPorAutor(string Autor);
        List<LibroModelo> ListLibroPorTitulo(string Titulo);
        ReviewModel ListReviewPorId(long id);
        long AddReview(LibroModelo Review);
        void SaveChanges();
    }
}
