using Dominio.Servicios.Contratos;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio.Servicios.Implementaciones
{
    public class EmailValidator : IValidate<string>
    {
        public bool Validate(string Correo)
        {
            string patronCorreo;
            Regex regex;
            if (string.IsNullOrEmpty(Correo))
            {
                throw new ArgumentException("Debe ingresar un correo!");

            }
            patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            regex = new Regex(patronCorreo);
            if (!regex.IsMatch(Correo))
            {
                throw new ArgumentException("Debe ingresar un correo válido.");
            }

            return true;
        }
    }
}
