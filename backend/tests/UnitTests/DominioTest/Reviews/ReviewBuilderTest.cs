using Dominio.Reviews.Builder;
using Dominio.Reviews.Modelo;
using Dominio.Libros.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioTest.Reviews
{
    internal class ReviewBuilderTest:ReviewModeloBuilder
    {
        private ReviewModel _review = new ReviewModel();
        public ReviewBuilderTest()
        {
            SetId(1)
            .SetComentario("Comentario amigable Pruebas")
            .SetCalificacion(3)
            .SetCreatedAt(DateTime.Now)
            .SetUsuario(new UsuarioModelo { Correo = "Correo@Prueba.com",Nombre = "Nombre Prueba", Password = "p4sSG@0d" })
            .SetLibro(new LibroModelo { Autor = "Autor Prueba", Titulo = "Titulo Prueba", Categoria = "Categoria Prueba", Resumen = "Resumen del libro de prueba" });
        }
    }
}
