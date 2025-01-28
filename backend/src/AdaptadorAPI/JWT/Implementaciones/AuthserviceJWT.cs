using AdaptadorAPI.Contratos;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Microsoft.IdentityModel.Tokens;

namespace AdaptadorAPI.Implementaciones
{
    public class AuthserviceJWT : IAuthService
    {
        private readonly IJwtService _jtwService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthserviceJWT(IJwtService jtwService, IRefreshTokenRepository refreshTokenRepo)
        {
            _jtwService = jtwService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public AuthenticationResult Authenticate(UsuarioModelo Usuario)
        {

            return GenerateTokens(Usuario);

        }
        public AuthenticationResult RefreshToken(string refreshToken)
        {
            // Buscar usuario por refresh token

            var usuario = _refreshTokenRepo.ListUsuarioByRefreshToken(refreshToken);

            // Generar nuevos tokens
            return GenerateTokens(usuario);

        }

        private AuthenticationResult GenerateTokens(UsuarioModelo usuario)
        {
            // Generar nuevos tokens
            string accessToken = _jtwService.GenerateAccessToken(usuario);
            string refreshToken = _jtwService.GenerateRefreshToken();
            DateTime expiry = DateTime.UtcNow.AddMinutes(_jtwService.GetRefreshTokenExpiration());

            // Actualizar el refresh token en la base de datos

            if (usuario.Id <= 0)
            {
                throw new ArgumentException("El ID del usuario no es válido.");
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("El nuevo Refresh Token del usuario no es válido.");
            }

            if (expiry < DateTime.UtcNow)
            {
                throw new ArgumentException("La nueva fehca de expiración del Refresh Token debe ser válida.");
            }

            _refreshTokenRepo.UpdateRefreshToken(usuario.Id, refreshToken, expiry);

            return new AuthenticationResult
            {
                Credential = accessToken,
                RenewalCredential = refreshToken,
                Expiry = expiry
            };
        }

        public void Logout(long id)
        {
            _refreshTokenRepo.UpdateRefreshToken(id, "", DateTime.MinValue);

        }
    }
}
