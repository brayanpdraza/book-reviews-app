using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Libros.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Reviews.Entidades
{
    public class ReviewEntity
    {
        public long Id { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime CreatedAt { get; set; }

        public long UsuarioId { get; set; }
        public UsuarioEntity Usuario { get; set; }

        public long LibroId { get; set; } 
        public LibroEntity Libro { get; set; }
    }
}
