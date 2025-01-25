using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Libros.Mappers;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Usuarios.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Adaptadores
{
    public class LibroRepositorioPostgreSQL : ILibroRepositorio
    {
        private readonly PostgreSQLDbContext _postgreSQLDbContext;
        private readonly MapToLibroEntity _mapToLibroEntity;
        private readonly MapToLibroModelDominio _mapToLibroModelDominio;

        public LibroRepositorioPostgreSQL(PostgreSQLDbContext dbContext)
        {
            _postgreSQLDbContext = dbContext;
            _mapToLibroEntity =  new MapToLibroEntity(dbContext);
            _mapToLibroModelDominio = new MapToLibroModelDominio();

        }

        public int ConteoLibros(string filtro = null)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros;

            // Aplicar filtro si se proporciona
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(l =>
                    l.Titulo.Contains(filtro) ||
                    l.Autor.Contains(filtro) ||
                    l.Resumen.Contains(filtro));
            }

            // Devolver el conteo total
            return query.Count();
        }

        public LibroModelo ListLibroPorId(long id)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros.Include(l => l.Categoria).Where(l => l.Id == id);

            LibroEntity libroEntity = query.FirstOrDefault();

            LibroModelo libro = _mapToLibroModelDominio.MapToLibroModelo(libroEntity);

            return libro;
        }

        public List<LibroModelo> ListLibrosPaginadosPorFiltroOpcional(int skip, int tamanoPagina, string filtro = null)
        {
            IQueryable<LibroEntity> query = _postgreSQLDbContext.Libros.Include(l => l.Categoria);

            // Aplicar filtro si se proporciona
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(l =>
                    l.Titulo.Contains(filtro) ||
                    l.Autor.Contains(filtro) ||
                    l.Resumen.Contains(filtro));
            }

            // Aplicar paginación
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
    }
}
