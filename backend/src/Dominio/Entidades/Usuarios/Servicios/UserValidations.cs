using Dominio.Servicios.Contratos;
using Dominio.Servicios.Implementaciones;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
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
        private readonly IValidate<UsuarioModelo> _usuarioValidator;
        public UserValidations()
        {
            _passwordValidator = new PasswordValidator();
            _emailValidator = new EmailValidator();
            _usuarioValidator = new UsuarioValidator();
        }

        public bool Validate(UsuarioModelo UsuarioValidar)
        {
            _usuarioValidator.Validate(UsuarioValidar);
            _emailValidator.Validate(UsuarioValidar.Correo);
            _passwordValidator.Validate(UsuarioValidar.Password);
            return true;

        }
    }


}
