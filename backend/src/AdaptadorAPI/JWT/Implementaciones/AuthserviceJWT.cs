using AdaptadorAPI.Contratos;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;

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
            string accessToken;
            string refreshToken;
            DateTime Expiry;
            int expirationMinutes;

            accessToken = _jtwService.GenerateAccessToken(Usuario);
            refreshToken = _jtwService.GenerateRefreshToken();
            expirationMinutes = _jtwService.GetRefreshTokenExpiration();
            Expiry = DateTime.Now.AddMinutes(expirationMinutes);
            _refreshTokenRepo.UpdateRefreshToken(Usuario.Id, refreshToken, Expiry);

            return new AuthenticationResult
            {
                Credential = accessToken,
                RenewalCredential = refreshToken,
                Expiry = Expiry
            };
        }
    }
}
