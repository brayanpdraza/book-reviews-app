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
    public class ReviewValidations
    {
        private readonly IValidate<int> _calificationBookValidator;
        private readonly IValidate<string> _commentValidator;
        private readonly IValidate<LibroModelo> _bookValidator;
        private readonly IValidate<UsuarioModelo> _UserValidator;
        private readonly ReviewModel _reviewValidar;
        public ReviewValidations(ReviewModel ReviewValidar)
        {
            _calificationBookValidator = new CalificacionLibroValidator();
            _commentValidator = new ComentarioValidator();
            _bookValidator = new LibroValidator();
            _UserValidator = new UsuarioValidator();
            _reviewValidar = ReviewValidar;
        }

        public bool Validate()
        {
            _calificationBookValidator.Validate(_reviewValidar.Calificacion);
            _commentValidator.Validate(_reviewValidar.Comentario);
            _bookValidator.Validate(_reviewValidar.Libro);
            _UserValidator.Validate(_reviewValidar.Usuario);
            return true;

        }
    }
}
