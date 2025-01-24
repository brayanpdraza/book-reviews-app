using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Servicios.Contratos;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Dominio.Usuarios.Servicios;

namespace Aplicacion.Libros
{
    public class UseCaseUsuario
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEncription _encription;
        private readonly UserValidations _userValidations;

        public UseCaseUsuario(IUsuarioRepositorio usuarioRepositorio, IEncription encriptacionClave, UserValidations userValidations)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encription = encriptacionClave;
            _userValidations = userValidations;
        }
        public long AddUsuario(UsuarioModelo usuario)
        {
            long idCreado;
            UsuarioModelo UsuarioExistente = new UsuarioModelo();

            _userValidations.Validate(usuario);

            UsuarioExistente = _usuarioRepositorio.ListUsuarioPorCorreo(usuario.Correo);
            if (UsuarioExistente != null && UsuarioExistente.Id != 0)
            {
                throw new Exception("El correo ingresado ya se encuentra Registrado en el sistema");
            }

            usuario.Password = _encription.Encriptar(usuario.Password);
            idCreado = _usuarioRepositorio.AddUsuario(usuario);

            return idCreado;
        }
        public UsuarioModelo ConsultarUsuarioCredenciales(string Correo, string Password)
        {
            UsuarioModelo usuario = new UsuarioModelo();
            if (string.IsNullOrEmpty(Correo))
            {
                throw new ArgumentNullException("Debe ingresar un Correo");
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentNullException("Debe ingresar una Contraseña");
            }
            usuario = _usuarioRepositorio.ListUsuarioPorCorreoPassword(Correo, Password);

            if(usuario == null || usuario.Id == 0)
            {
                throw new InvalidCredentialException("Credenciales Incorrectas");
            }

            return usuario;
        }
    }
}
