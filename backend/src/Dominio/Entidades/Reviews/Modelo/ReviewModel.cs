using Dominio.Libros.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Reviews.Modelo
{
    public class ReviewModel
    {
        public long Id { get; set; }
        public int Calificacion { get; set; } 
        public string Comentario { get; set; } 
        public DateTime CreatedAt { get; set; }

        private UsuarioModelo _usuario;
        public UsuarioModelo Usuario
        {
            get => _usuario ??= new UsuarioModelo();
            set => _usuario = value;
        }

        private LibroModelo _libro;
        public LibroModelo Libro
        {
            get => _libro ??= new LibroModelo();
            set => _libro = value;
        }
    }
}
