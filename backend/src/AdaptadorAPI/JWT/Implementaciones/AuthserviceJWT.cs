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
            (UsuarioModelo usuario, AuthenticationResult authResult) = _refreshTokenRepo.ListUsuarioByRefreshToken(refreshToken);

            if (usuario.Id <= 0)
            {
                throw new UnauthorizedAccessException("El Refresh Token No se encuentra asociado a ningún usuario.");
            }

            if (authResult.Expiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("El Refresh Token se ha vencido. Debe realizar un nuevo inicio de sesión.");
            }

            return GenerateTokens(usuario);

        }

        private AuthenticationResult GenerateTokens(UsuarioModelo usuario)
        {
            // Generar nuevos tokens
            string accessToken = _jtwService.GenerateAccessToken(usuario);
            string refreshToken = _jtwService.GenerateRefreshToken();
            DateTime ExpirySystemTime = DateTime.UtcNow.AddMinutes(_jtwService.GetRefreshTokenExpiration());

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedAccessException("El nuevo Refresh Token del usuario no es válido.");
            }

            // Actualizar el refresh token en la base de datos
            _refreshTokenRepo.UpdateRefreshToken(usuario.Id, refreshToken, ExpirySystemTime);

            return new AuthenticationResult
            {
                Credential = accessToken,
                RenewalCredential = refreshToken,
                Expiry = ExpirySystemTime
            };
        }

        public void Logout(long id)
        {
            _refreshTokenRepo.UpdateRefreshToken(id, "", DateTime.MinValue);

        }
    }
}
