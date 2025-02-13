using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.Contratos;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Reviews.Servicios
{
    public class ReviewPartialUpdateValidations:IReviewPartialUpdateValidations
    {
        private readonly IDictionary<string, IValidate<object>> _validators;

        public ReviewPartialUpdateValidations()
        {
            // El diccionario usa claves en mayúscula para uniformidad.
            _validators = new Dictionary<string, IValidate<object>>(StringComparer.OrdinalIgnoreCase)
            {
                // Usamos adaptadores para convertir el IValidate<T> a IValidate<object>
                { "CALIFICACION", new ObjectValidatorAdapter<int>(new CalificacionLibroValidator()) },
                { "COMENTARIO", new ObjectValidatorAdapter<string>(new ComentarioValidator()) }

            };

        }
        public bool Validate(string Key, object Value)
        {
            if (!_validators.ContainsKey(Key))
            {
                throw new Exception($"El campo '{Key}' no está permitido para modificación.");
            }

            return _validators[Key].Validate(Value);
        }

    }
}
