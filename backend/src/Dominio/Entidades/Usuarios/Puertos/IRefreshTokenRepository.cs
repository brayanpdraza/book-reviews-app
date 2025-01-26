using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Usuarios.Puertos
{
    public interface IRefreshTokenRepository
    {
        UsuarioModelo ListUsuarioByRefreshToken(string refreshToken);
        void UpdateRefreshToken(long usuarioId, string refreshToken, DateTime expiry);
        void SaveChanges();
    }
}
