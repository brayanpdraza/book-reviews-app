using AdaptadorPostgreSQL.CategoriasLibro.Entidades;
using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Reviews.Mappers
{
    public class MapToReviewEntity
    {
        private readonly PostgreSQLDbContext _dbContext;
        public MapToReviewEntity(PostgreSQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ReviewEntity MapToReviewEntidad(ReviewModel modelDominio)
        {
            if (modelDominio == null)
            {
                throw new ArgumentNullException(nameof(modelDominio), "El modelo de dominio no puede ser nulo.");
            }

            UsuarioEntity UsuarioEntity = _dbContext.Usuarios.FirstOrDefault(c => c.Id == modelDominio.Usuario.Id);

            if (UsuarioEntity == null)
            {
                throw new InvalidOperationException($"No se ha encontrado un usuario relacionado al id: '{modelDominio.Usuario.Id}'.");
            }

            LibroEntity LibroEntity = _dbContext.Libros.FirstOrDefault(c => c.Id == modelDominio.Libro.Id);

            if (UsuarioEntity == null)
            {
                throw new InvalidOperationException($"No se ha encontrado un usuario relacionado al id: '{modelDominio.Libro.Id}'.");
            }

            return new ReviewEntity
            {
                Id = modelDominio.Id,
                Calificacion = modelDominio.Calificacion,
                Comentario = modelDominio.Comentario,
                CreatedAt = modelDominio.CreatedAt,
                Usuario = UsuarioEntity,
                UsuarioId = LibroEntity.Id,
                Libro = LibroEntity,
                LibroId = LibroEntity.Id
            };
        }
    }
}
