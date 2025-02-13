using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Contratos
{
    public interface IpropertyModelValidate
    {
        bool ValidarPropiedad<T>(string nombrePropiedad);
    }
}
