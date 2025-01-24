using Dominio.Entidades.Libros.Builder;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionTest.Libros
{
    internal class LibrosBuilderUseCaseTest:LibroModeloBuilder
    {
        private UsuarioModelo _usuario = new UsuarioModelo();
        public LibrosBuilderUseCaseTest()
        {
            SetId(1)
            .SetCategoria("Categoria Prueba")
            .SetAutor("Autor Prueba")
            .SetTitulo("Titulo Prueba")
            .SetResumen("Resumen Libro Prueba");
        }
    }
}
