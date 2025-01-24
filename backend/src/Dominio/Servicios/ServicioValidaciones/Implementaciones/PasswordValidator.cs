using Dominio.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio.Servicios.Implementaciones
{
    public class PasswordValidator : IValidate<string>
    {
        public bool Validate(string Password)
        {
            Regex regex;
            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException("Debe ingresar una contraseña!");

            }
            if (Password.Length < 8)
            {
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
            }
            regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            if (!regex.IsMatch(Password))
            {
                throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial.");
            }

            return true;
        }
    }
}
