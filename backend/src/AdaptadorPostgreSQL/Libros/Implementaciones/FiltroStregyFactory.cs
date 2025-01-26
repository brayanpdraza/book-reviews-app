using AdaptadorPostgreSQL.Libros.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Implementaciones
{
    public class FiltroStregyFactory
    {
        private readonly Dictionary<string, IFiltroStrategy> _strategies;

        public FiltroStregyFactory()
        {
            _strategies = new Dictionary<string, IFiltroStrategy>
        {
            { "titulo", new FiltroTituloStrategy() },
            { "autor", new FiltroAutorStrategy() },
            { "categoria", new FiltroCategoriaStrategy() },
            { "", new FiltroVacioStrategy() } // Para búsqueda general
            };
        }

        public IFiltroStrategy GetStrategy(string tipoFiltro)
        {
            if (_strategies.ContainsKey(tipoFiltro))
            {
                return _strategies[tipoFiltro];
            }
            throw new ArgumentException($"Campo de filtro no válido: '{tipoFiltro}'.");
        }
    }
}
