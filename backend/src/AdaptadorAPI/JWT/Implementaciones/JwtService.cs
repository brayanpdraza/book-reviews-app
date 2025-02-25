using AdaptadorAPI.Contratos;
using Dominio.Usuarios.Modelo;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdaptadorAPI.Implementaciones
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtService(IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }
        public (string AccessToken, string RefreshToken) GenerateTokens(UsuarioModelo usuario)
        {
            var accessToken = GenerateAccessToken(usuario);
            var refreshToken = GenerateRefreshToken();
            return (accessToken, refreshToken);
        }

        public string GenerateAccessToken(UsuarioModelo usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetAccessSecretKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim("id", usuario.Id.ToString()),
            new Claim("correo", usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
            issuer: GetAccessIssuer(),
            audience: GetAccessAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string accessToken)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Token inválido");

            return principal;

        }
        public string GetAccessSecretKey() => Environment.GetEnvironmentVariable("Jwt_SecretKey");
        public string GetAccessIssuer() => Environment.GetEnvironmentVariable("Jwt_Issuer");
        public string GetAccessAudience() => Environment.GetEnvironmentVariable("Jwt_Audience");
        public int GetAccessTokenExpiration() => Convert.ToInt32(Environment.GetEnvironmentVariable("Jwt_AccessTokenExpiration"));
        public int GetRefreshTokenExpiration() => Convert.ToInt32(Environment.GetEnvironmentVariable("Jwt_RefreshTokenExpiration"));
        public string GetJtiFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Id;
        }

    }
}
