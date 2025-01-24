using Dominio.Libros.Modelo;
using Dominio.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class LibroValidator : IValidate<LibroModelo>
    {
        public bool Validate(LibroModelo Libro)
        {
            if (Libro == null)
            {
                throw new ArgumentException("El libro no puede ser nulo.");
            }
            return true;
        }
    }
}
