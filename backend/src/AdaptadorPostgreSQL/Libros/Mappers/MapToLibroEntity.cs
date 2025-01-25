using AdaptadorPostgreSQL.CategoriasLibro.Entidades;
using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Libros.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Libros.Mappers
{
    public class MapToLibroEntity
    {
        private readonly PostgreSQLDbContext _dbContext;
        public MapToLibroEntity(PostgreSQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LibroEntity MapToLibroEntidad(LibroModelo modelDominio)
        {
            if (modelDominio == null)
            {
                throw new ArgumentNullException(nameof(modelDominio), "El modelo de dominio no puede ser nulo.");
            }

            CategoriasLibroEntity categoriaEntity = _dbContext.CategoriasLibro.FirstOrDefault(c => c.Nombre == modelDominio.Categoria);

            if (categoriaEntity == null)
            {
                throw new InvalidOperationException($"La categoría '{modelDominio.Categoria}' no existe.");
            }

            return new LibroEntity
            {
                Id = modelDominio.Id,
                Titulo = modelDominio.Titulo,
                Autor = modelDominio.Autor,
                Resumen = modelDominio.Resumen,
                CategoriasLibroId = categoriaEntity.id, 
                Categoria = categoriaEntity,         
                Reviews = new List<ReviewEntity>()     
            };
        }
    }
}
