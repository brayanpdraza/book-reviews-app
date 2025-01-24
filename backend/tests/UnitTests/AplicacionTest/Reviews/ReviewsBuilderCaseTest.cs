using Dominio.Entidades.Reviews.Puertos;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Builder;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionTest.Reviews
{
    public class ReviewsBuilderCaseTest:ReviewModeloBuilder
    {
        private ReviewModel _review = new ReviewModel();
        private IReviewValidations _reviewValidations;
        public ReviewsBuilderCaseTest(IReviewValidations reviewValidations) : base(reviewValidations)
        {
            _reviewValidations = reviewValidations;
            SetId(1)
           .SetComentario("Comentario amigable Pruebas")
           .SetCalificacion(3)
           .SetCreatedAt(DateTime.Now)
           .SetUsuario(new UsuarioModelo { Id=1,Correo = "Correo@Prueba.com", Nombre = "Nombre Prueba", Password = "p4sSG@0d" })
           .SetLibro(new LibroModelo { Id=1,Autor = "Autor Prueba", Titulo = "Titulo Prueba", Categoria = "Categoria Prueba", Resumen = "Resumen del libro de prueba" });
        }
    }
}
