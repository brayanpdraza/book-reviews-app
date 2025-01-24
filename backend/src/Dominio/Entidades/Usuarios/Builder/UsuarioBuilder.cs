using Dominio.Reviews.Builder;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios.Builder
{
    public abstract class UsuarioBuilder
    {
        private UsuarioModelo _modelo = new UsuarioModelo();
        private UserValidations _userValidations;

        public UsuarioBuilder SetId(long ID)
        {
            _modelo.Id = ID;
            return this;
        }

        public UsuarioBuilder SetNombre(string nombre)
        {
            _modelo.Nombre = nombre;
            return this;
        }

        public UsuarioBuilder SetCorreo(string correo)
        {
            _modelo.Correo = correo;
            return this;
        }

        public UsuarioBuilder SetPassword(string password)
        {
            _modelo.Password = password;
            return this;
        }

        public bool Validate()
        {
            _userValidations.Validate(_modelo);
            return true;

        }

        public UsuarioModelo Build()
        {
            {
                return _modelo;
            }
        }
    }
}
