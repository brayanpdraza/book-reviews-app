using Dominio.Servicios.Contratos;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class CalificacionLibroValidator : IValidate<int>
    {
        public bool Validate(int Calificacion)
        {
            if (Calificacion < 1 || Calificacion > 5)
            {
                throw new ArgumentException("La calificación debe estar entre 1 y 5.");
            }
            return true;
        }
    
    }
}
