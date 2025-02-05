using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Usuarios.Mappers
{
    public class MapToUsuarioEntity
    {
        public UsuarioEntity MapToUsusarioEntidad(UsuarioModelo ModelDominio)
        {
            if (ModelDominio == null)
            {
                throw new ArgumentException(nameof(ModelDominio), "El modelo de dominio no puede ser nulo.");
            }

            return new UsuarioEntity
            {
                Id = ModelDominio.Id,
                Nombre = ModelDominio.Nombre,
                FotoPerfil = ModelDominio.FotoPerfil,
                Correo = ModelDominio.Correo,
                Password = ModelDominio.Password,
                Reviews = new List<ReviewEntity>()
            };
        }
    }
}
