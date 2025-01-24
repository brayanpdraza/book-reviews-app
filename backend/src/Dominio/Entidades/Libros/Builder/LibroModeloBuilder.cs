using Dominio.Libros.Modelo;
using Dominio.Usuarios.Builder;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Libros.Builder
{
    public class LibroModeloBuilder
    {
        private LibroModelo _modelo = new LibroModelo();
        public LibroModeloBuilder SetId(long ID)
        {
            _modelo.Id = ID;
            return this;
        }

        public LibroModeloBuilder SetTitulo(string titulo)
        {
            _modelo.Titulo = titulo;
            return this;
        }

        public LibroModeloBuilder SetAutor(string autor)
        {
            _modelo.Autor = autor;
            return this;
        }

        public LibroModeloBuilder SetCategoria(string categoria)
        {
            _modelo.Categoria = categoria;
            return this;
        }

        public LibroModeloBuilder SetResumen(string resumen)
        {
            _modelo.Resumen = resumen;
            return this;
        }

        public LibroModelo Build()
        {
            {
                return _modelo;
            }
        }
    }
}
