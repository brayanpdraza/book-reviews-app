using Dominio.Servicios.Contratos;
using Dominio.Servicios.Implementaciones;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios.Servicios
{
    public class UserValidations
    {
        private readonly IValidate<string> _passwordValidator;
        private readonly IValidate<string> _emailValidator;
        private readonly UsuarioModelo _usuarioValidar;
        public UserValidations(UsuarioModelo UsuarioValidar)
        {
            _passwordValidator = new PasswordValidator();
            _emailValidator = new EmailValidator();
            _usuarioValidar = UsuarioValidar; 
        }

        public bool Validate()
        {
            _emailValidator.Validate(_usuarioValidar.Correo);
            _passwordValidator.Validate(_usuarioValidar.Password);
            return true;

        }
    }


}
