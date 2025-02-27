using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Usuarios.Adaptadores
{
    public class RefreshTokenPostgreSQL : IRefreshTokenRepository
    {
        private readonly PostgreSQLDbContext _postgreSQLDbContext;
        private readonly MapToUserModelDominio _mapToUserModelDominio;
        private readonly MapToUsuarioEntity _mapToUserEntity;

        public RefreshTokenPostgreSQL(PostgreSQLDbContext postgreSQLDbContext)
        {
            _postgreSQLDbContext= postgreSQLDbContext;
            _mapToUserModelDominio = new MapToUserModelDominio();
            _mapToUserEntity = new MapToUsuarioEntity();
        }
        public (UsuarioModelo, AuthenticationResult) ListUsuarioByRefreshToken(string refreshToken)
        {
            UsuarioModelo usuario;
            UsuarioEntity usuarioEntity = _postgreSQLDbContext.Usuarios.FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (usuarioEntity == null)
            {
                return(new UsuarioModelo(), new AuthenticationResult());
            }

            usuario = _mapToUserModelDominio.MapToUserDomainModel(usuarioEntity);

            AuthenticationResult authResult = new AuthenticationResult
            {
                Credential = "JWT_GENERADO_AQUÍ",
                RenewalCredential = usuarioEntity.RefreshToken,
                Expiry = usuarioEntity.RefreshTokenExpiry 
            };

            return (usuario,authResult);
        }

        public void UpdateRefreshToken(UsuarioModelo usuario, string refreshToken, DateTime expiry)
        {
            UsuarioEntity usuarioEntity = _mapToUserEntity.MapToUsusarioEntidad(usuario);

            usuarioEntity.RefreshToken = refreshToken;
            usuarioEntity.RefreshTokenExpiry = expiry;

            // Verificar si ya existe una instancia rastreada con el mismo Id
            var existingEntity = _postgreSQLDbContext.Usuarios
                .Local
                .FirstOrDefault(r => r.Id == usuarioEntity.Id);

            if (existingEntity != null)
            {
                // Desconectar la instancia existente
                _postgreSQLDbContext.Entry(existingEntity).State = EntityState.Detached;
            }

            _postgreSQLDbContext.Usuarios.Update(usuarioEntity);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }


    }

}
