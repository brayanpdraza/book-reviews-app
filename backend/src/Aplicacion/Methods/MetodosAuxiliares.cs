using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Methods
{
    public class MetodosAuxiliares
    {
        public int TotalPaginas(int totalRegistros, int tamanoPagina)
        {
            if (tamanoPagina == 0)
            {
                throw new DivideByZeroException("El tamano de Pagina no puede ser 0");
            }

            return (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
        }
    }
}
