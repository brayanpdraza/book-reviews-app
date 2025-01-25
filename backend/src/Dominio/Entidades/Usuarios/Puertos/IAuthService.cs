using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Usuarios.Modelo;

namespace Dominio.Usuarios.Puertos
{
    public interface IAuthService
    {
        AuthenticationResult Authenticate(UsuarioModelo usuarioModelo);
    }
}
