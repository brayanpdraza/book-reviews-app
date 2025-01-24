using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Servicios.Contratos;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Dominio.Usuarios.Servicios;

namespace Aplicacion.Usuarios
{
    public class UseCaseUsuario
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEncription _encription;
        private readonly IUserValidations _userValidations;

        public UseCaseUsuario(IUsuarioRepositorio usuarioRepositorio, IEncription encriptacionClave, IUserValidations userValidations)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encription = encriptacionClave;
            _userValidations = userValidations;
        }
        public long AddUsuario(UsuarioModelo usuario)
        {
            long idCreado;
            UsuarioModelo UsuarioExistente;

            _userValidations.Validate(usuario);

            UsuarioExistente = _usuarioRepositorio.ListUsuarioPorCorreo(usuario.Correo);
            if (UsuarioExistente != null && UsuarioExistente.Id > 0)
            {
                throw new Exception("El correo ingresado ya se encuentra Registrado en el sistema.");
            }

            usuario.Password = _encription.Encriptar(usuario.Password);
            idCreado = _usuarioRepositorio.AddUsuario(usuario);

            if (idCreado <= 0)
            {
                throw new Exception("El usuario no ha sido creado.");
            }

            return idCreado;
        }
        public UsuarioModelo ConsultarUsuarioCredenciales(string Correo, string Password)
        {
            UsuarioModelo usuario;
            bool esValidoPass;

            if (string.IsNullOrEmpty(Correo))
            {
                throw new ArgumentException("Debe ingresar un Correo.");
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException("Debe ingresar una Contraseña.");
            }

            usuario = _usuarioRepositorio.ListUsuarioPorCorreo(Correo);

            if (usuario == null || usuario.Id == 0)
            {
                throw new Exception("El correo ingresado no se encuentra Registrado.");
            }

            esValidoPass = _encription.VerificarClaveEncriptada(usuario.Password, Password);

            if (!esValidoPass)
            {
                throw new Exception("La contraseña proporcionada es Incorrecta.");
            }

            return usuario;
        }
    }
}
