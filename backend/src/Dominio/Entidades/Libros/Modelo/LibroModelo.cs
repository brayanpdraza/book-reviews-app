using Dominio.Reviews.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Libros.Modelo
{
    public class LibroModelo
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Autor { get; set; }
        public string Resumen { get; set; }

        public ICollection<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
}
