using Dominio.Usuarios.Modelo;
using System.Security.Claims;

namespace AdaptadorAPI.Contratos
{
    public interface IJwtService
    {
        string GenerateAccessToken(UsuarioModelo usuario);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
        int GetAccessTokenExpiration();
        int GetRefreshTokenExpiration();
        string GetJtiFromToken(string Token);
    }
}
