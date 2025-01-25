using AdaptadorPostgreSQL.CategoriasLibro.Entidades;
using AdaptadorPostgreSQL.Reviews.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Entidades
{
    public class LibroEntity
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Resumen { get; set; }
        
        public long CategoriasLibroId { get; set; }
        public CategoriasLibroEntity Categoria { get; set; }

        public ICollection<ReviewEntity> Reviews { get; set; }
    }
}
