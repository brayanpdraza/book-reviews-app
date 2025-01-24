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
        private UsuarioModelo _usuario = new UsuarioModelo();
        public UsuarioBuilderTest()
        {
            SetId(1)
            .SetNombre("Nombre Pruebas")
            .SetCorreo("Correo@Pruebas.com")
            .SetPassword("P4ssG@0d");
        }
    }
}
