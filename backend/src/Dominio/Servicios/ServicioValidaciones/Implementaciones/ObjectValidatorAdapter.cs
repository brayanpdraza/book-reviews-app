using Dominio.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class ObjectValidatorAdapter<T> : IValidate<object>
    {
        private readonly IValidate<T> _innerValidator;

        public ObjectValidatorAdapter(IValidate<T> innerValidator)
        {
            _innerValidator = innerValidator;
        }

        public bool Validate(object value)
        {
            // Intenta convertir el valor al tipo esperado
            if(value == null)
            {
                throw new Exception($"Error de validación: El valor no puede ser nulo");
            }
            if (value is T tValue)
            {
                return _innerValidator.Validate(tValue);
            }
            else
            {
                throw new Exception($"Error de validación: Se esperaba un valor de tipo '{typeof(T).Name}', pero se recibió '{value.GetType().Name}'.");
            }
        }
    }
}
