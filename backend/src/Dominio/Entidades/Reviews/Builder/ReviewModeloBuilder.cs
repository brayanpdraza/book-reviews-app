using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Reviews.Servicios;
using Dominio.Usuarios.Builder;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Reviews.Builder
{
    public abstract class ReviewModeloBuilder
    {
        private ReviewModel _modelo = new ReviewModel();
        private ReviewValidations _reviewValidations;

        public ReviewModeloBuilder SetId(long ID)
        {
            _modelo.Id = ID;
            return this;
        }

        public ReviewModeloBuilder SetCalificacion(int Calificacion)
        {
            _modelo.Calificacion = Calificacion;
            return this;
        }

        public ReviewModeloBuilder SetComentario(string Comentario)
        {
            _modelo.Comentario = Comentario;
            return this;
        }

        public ReviewModeloBuilder SetCreatedAt(DateTime FechaCreacion)
        {
            _modelo.CreatedAt = FechaCreacion;
            return this;
        }

        public ReviewModeloBuilder SetLibro(LibroModelo Libro)
        {
            _modelo.Libro = Libro;
            return this;
        }

        public ReviewModeloBuilder SetUsuario(UsuarioModelo usuario)
        {
            _modelo.Usuario = usuario;
            return this;
        }


        public bool Validate()
        {
            _reviewValidations.Validate();
            return true;

        }

        public ReviewModel Build()
        {
            {
                return _modelo;
            }
        }
    }
}
