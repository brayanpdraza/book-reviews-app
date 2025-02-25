using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Modelo;
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

        public RefreshTokenPostgreSQL(PostgreSQLDbContext postgreSQLDbContext)
        {
            _postgreSQLDbContext= postgreSQLDbContext;
            _mapToUserModelDominio = new MapToUserModelDominio();
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

        public void UpdateRefreshToken(long usuarioId, string refreshToken, DateTime expiry)
        {

            UsuarioEntity usuario = _postgreSQLDbContext.Usuarios.Find(usuarioId);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuario No registrado. No puede Autenticarse.");

            }

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiry = expiry;
            SaveChanges();
        }

        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }


    }

}
