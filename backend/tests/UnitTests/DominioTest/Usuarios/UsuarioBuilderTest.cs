using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Builder;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominioTest.Usuarios
{
    internal class UsuarioBuilderTest : UsuarioBuilder
    {
        private IUserValidations _userValidations;
        private UsuarioModelo _usuario;

        public UsuarioBuilderTest(IUserValidations userValidations):base(userValidations)
        {
            _userValidations = userValidations;
            SetId(1)
          .SetNombre("Nombre Pruebas")
          .SetCorreo("Correo@Pruebas.com")
          .SetPassword("P4ssG@0d");
        }


    }
}
