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

        public UsuarioModelo Usuario { get; set; }

        public LibroModelo Libro {get ; set; }
    }
}
