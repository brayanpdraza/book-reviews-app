using AdaptadorPostgreSQL.Libros.Contratos;
using AdaptadorPostgreSQL.Libros.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Implementaciones
{
    public class FiltroAutorStrategy:IFiltroStrategy
    {
        public IQueryable<LibroEntity> AplicarFiltro(IQueryable<LibroEntity> query, string valor)
        {
            return query.Where(l => l.Autor.Contains(valor.ToUpper()));
        }
    }
}
