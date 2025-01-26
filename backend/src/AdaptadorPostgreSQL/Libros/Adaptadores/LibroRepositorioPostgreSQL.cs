using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Libros.Implementaciones;
using AdaptadorPostgreSQL.Libros.Mappers;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Libros.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdaptadorPostgreSQL.Libros.Adaptadores
{
    public class LibroRepositorioPostgreSQL : ILibroRepositorio
    {
        private readonly PostgreSQLDbContext _postgreSQLDbContext;
        private readonly MapToLibroModelDominio _mapToLibroModelDominio;

        public LibroRepositorioPostgreSQL(PostgreSQLDbContext dbContext)
        {
            _postgreSQLDbContext = dbContext;
            _mapToLibroModelDominio = new MapToLibroModelDominio();

        }

        public int ConteoLibros(string filtro = null)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros;

            query = GetQueryFiltro(query, filtro);

            return query.Count();
        }

        public LibroModelo ListLibroPorId(long id)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros.Include(l => l.Categoria).Where(l => l.Id == id);

            LibroEntity libroEntity = query.FirstOrDefault();

            if(libroEntity == null)
            {
                return new LibroModelo();
            }

            LibroModelo libro = _mapToLibroModelDominio.MapToLibroModelo(libroEntity);

            return libro;
        }

        public List<LibroModelo> ListLibrosPaginadosPorFiltroOpcional(int skip, int tamanoPagina, string filtro = null)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros.Include(l => l.Categoria);

            query = GetQueryFiltro(query, filtro);

            // Paginación
            List<LibroEntity> librosEntities = query
                .OrderBy(l => l.Titulo)
                .Skip(skip)
                .Take(tamanoPagina)
                .ToList();

            return _mapToLibroModelDominio.MapToLibroModeloList(librosEntities);


        }

        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }

        private (string tipoFiltro, string valor) ParsearFiltro(string filtro)
        {
            if (string.IsNullOrEmpty(filtro))
            {
                return ("", filtro);
            }

            var partes = filtro.Split(new[] { ':' }, 2);
            if (partes.Length != 2 || string.IsNullOrWhiteSpace(partes[1]))
            {
                throw new ArgumentException("Formato de filtro inválido. Use 'campo:valor'.");
            }

            string tipo = partes[0].Trim().ToLower();
            string valor = partes[1].Trim();

            return (tipo, valor);
        }

        private IQueryable<LibroEntity> GetQueryFiltro(IQueryable<LibroEntity> query, string filtro)
        {
            var (tipoFiltro, valor) = ParsearFiltro(filtro);
            var factory = new FiltroStregyFactory();
            var strategy = factory.GetStrategy(tipoFiltro);
            return strategy.AplicarFiltro(query, valor);
        }
    }
}
