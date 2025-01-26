using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Usuarios.Modelo;

namespace Dominio.Usuarios.Puertos
{
    public interface IAuthService
    {
        AuthenticationResult Authenticate(UsuarioModelo usuarioModelo);
        AuthenticationResult RefreshToken(string refreshToken);
        void Logout(long id);
    }
}
