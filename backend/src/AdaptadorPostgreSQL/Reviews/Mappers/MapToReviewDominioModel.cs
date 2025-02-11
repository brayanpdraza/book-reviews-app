using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Reviews.Entidades;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Builder;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Reviews.Mappers
{
    public class MapToReviewDominioModel
    {
        
        public ReviewModel MapToReviewdomainmodel(ReviewEntity entity)
        {
            return new ReviewModel
            {
                Id = entity.Id,
                Calificacion = entity.Calificacion,
                Comentario = entity.Comentario,
                CreatedAt = entity.CreatedAt,
                Usuario = new UsuarioModelo
                {
                    Id = entity.Usuario.Id,
                    Nombre = entity.Usuario.Nombre,
                    Correo = entity.Usuario.Correo,
                    Password = entity.Usuario.Password,
                    FotoPerfil = entity.Usuario.FotoPerfil,
                },
                Libro = new LibroModelo
                {
                    Id = entity.Libro.Id,
                    Categoria = entity.Libro.Categoria.Nombre,
                    Titulo = entity.Libro.Titulo,
                    Autor = entity.Libro.Autor,
                    Resumen = entity.Libro.Resumen
                }
            };
        }

        public List<ReviewModel> MapToReviewModeloList(List<ReviewEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "La lista de reseñas no puede ser nula.");
            }

            return entities.Select(entity => MapToReviewdomainmodel(entity)).ToList();
        }
    }
}
