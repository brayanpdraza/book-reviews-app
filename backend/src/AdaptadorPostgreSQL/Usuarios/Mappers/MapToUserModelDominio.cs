using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Usuarios.Mappers
{
    public class MapToUserModelDominio
    {
        public UsuarioModelo MapToUserDomainModel(UsuarioEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity), "La entidad no puede ser nula.");
            }
            return new UsuarioModelo
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                FotoPerfil = entity.FotoPerfil,
                Correo = entity.Correo,
                Password = entity.Password,
            };
        }
    }
}
