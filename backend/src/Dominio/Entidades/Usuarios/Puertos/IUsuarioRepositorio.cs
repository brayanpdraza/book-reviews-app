using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios.Puertos
{
    public interface IUsuarioRepositorio
    {
        List<UsuarioModelo> ListUsuariosAll();
        UsuarioModelo ListUsuarioPorId(long id);
        UsuarioModelo ListUsuarioPorCorreo(string Correo);
        UsuarioModelo ListUsuarioPorCorreoPassword(string Correo,string Password);
        long AddUsuario(UsuarioModelo usuario);
        void SaveChanges();
    }
}
