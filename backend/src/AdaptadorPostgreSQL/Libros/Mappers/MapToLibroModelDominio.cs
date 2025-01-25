using AdaptadorPostgreSQL.Libros.Entidades;
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
    public class MapToLibroModelDominio
    {
        public LibroModelo MapToLibroModelo(LibroEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity), "La entidad no puede ser nula.");
            }
            return new LibroModelo
            {
                Id = entity.Id,
                Categoria = entity.Categoria.Nombre,
                Autor = entity.Autor,
                Resumen = entity.Resumen,
                Titulo = entity.Titulo
            };
        }

        public List<LibroModelo> MapToLibroModeloList(List<LibroEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "La lista de lbros no puede ser nula.");
            }

            return entities.Select(entity => MapToLibroModelo(entity)).ToList(); 
        }
    }
}
