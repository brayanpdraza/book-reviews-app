using AdaptadorPostgreSQL.Libros.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Contratos
{
    public interface IFiltroStrategy
    {
        IQueryable<LibroEntity> AplicarFiltro(IQueryable<LibroEntity> query, string valor);
    }
}
