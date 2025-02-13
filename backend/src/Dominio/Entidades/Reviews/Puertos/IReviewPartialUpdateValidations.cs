using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Reviews.Puertos
{
    public interface IReviewPartialUpdateValidations
    {
        bool Validate(string Key, object Value);
    }
}
