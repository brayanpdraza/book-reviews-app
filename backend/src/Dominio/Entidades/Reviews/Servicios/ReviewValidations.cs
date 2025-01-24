using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.Contratos;
using Dominio.Servicios.Implementaciones;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Reviews.Servicios
{
    public class ReviewValidations:IReviewValidations
    {
        private readonly IValidate<int> _calificationBookValidator;
        private readonly IValidate<string> _commentValidator;
        private readonly IValidate<LibroModelo> _bookValidator;
        private readonly IValidate<UsuarioModelo> _UserValidator;
        public ReviewValidations()
        {
            _calificationBookValidator = new CalificacionLibroValidator();
            _commentValidator = new ComentarioValidator();
            _bookValidator = new LibroValidator();
            _UserValidator = new UsuarioValidator();
        }

        public bool Validate(ReviewModel ReviewValidar)
        {
            _calificationBookValidator.Validate(ReviewValidar.Calificacion);
            _commentValidator.Validate(ReviewValidar.Comentario);
            _bookValidator.Validate(ReviewValidar.Libro);
            _UserValidator.Validate(ReviewValidar.Usuario);
            return true;

        }
    }
}
