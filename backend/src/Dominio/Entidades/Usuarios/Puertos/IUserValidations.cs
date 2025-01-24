using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Usuarios.Puertos
{
    public interface IUserValidations
    {
        bool Validate(UsuarioModelo usuario);
    }
}
