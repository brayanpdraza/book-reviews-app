using Dominio.Servicios.Contratos;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class UsuarioValidator : IValidate<UsuarioModelo>
    {
        public bool Validate(UsuarioModelo Usuario)
        {
            if (Usuario == null)
            {
                throw new ArgumentException("El usuario no puede ser nulo.");
            }
            return true;
        }
    
    }
}
