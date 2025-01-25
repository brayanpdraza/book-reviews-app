using AdaptadorPostgreSQL.Libros.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.CategoriasLibro.Entidades
{
    public class CategoriasLibroEntity
    {
        public long id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<LibroEntity> libroEntities { get; set; }
    }
}
