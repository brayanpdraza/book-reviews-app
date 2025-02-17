using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;

namespace Aplicacion.Usuarios
{
    public class UseCaseUsuario
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEncription _encription;
        private readonly IUserValidations _userValidations;
        private readonly IAuthService _authService;

        public UseCaseUsuario(IUsuarioRepositorio usuarioRepositorio, IEncription encriptacionClave, IUserValidations userValidations, IAuthService authService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encription = encriptacionClave;
            _userValidations = userValidations;
            _authService = authService;
        }
        public long AddUsuario(UsuarioModelo usuario)
        {
            long idCreado;
            UsuarioModelo UsuarioExistente;

            _userValidations.Validate(usuario);

            UsuarioExistente = _usuarioRepositorio.ListUsuarioPorCorreo(usuario.Correo);
            if (UsuarioExistente.Id >0)
            {
                throw new DataException("El correo ingresado ya se encuentra Registrado en el sistema.");
            }

            usuario.Password = _encription.Encriptar(usuario.Password);
            idCreado = _usuarioRepositorio.AddUsuario(usuario);

            if (idCreado <= 0)
            {
                throw new Exception("El usuario no ha sido creado.");
            }

            return idCreado;
        }

        public UsuarioModelo ConsultarUsuarioPorId(long id)
        {
            UsuarioModelo Usuario;
            if (id <= 0)
            {
                throw new ArgumentException("No se puede consultar el usuario porque el id no es válido.");
            }

            Usuario = _usuarioRepositorio.ListUsuarioPorId(id);
            if (Usuario.Id <= 0)
            {
                throw new KeyNotFoundException($"El id {id} no se encuentra asociado a un usuario.");
            }

            return Usuario;
        }


        public AuthenticationResult AutenticacionByCredenciales(string Correo, string Password)
        {
            UsuarioModelo usuario;
            AuthenticationResult authenticationResult;
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
                throw new KeyNotFoundException("El correo ingresado no se encuentra Registrado.");
            }

            esValidoPass = _encription.VerificarClaveEncriptada(Password, usuario.Password);

            if (!esValidoPass)
            {
                throw new UnauthorizedAccessException("La contraseña proporcionada es Incorrecta.");
            }

            authenticationResult = _authService.Authenticate(usuario);

            if (authenticationResult == null)
            {
                throw new UnauthorizedAccessException("No se realizó la autenticacion.");
            }

            return authenticationResult;
        }

        public AuthenticationResult UpdateRefreshToken(string RefreshToken)
        {
            AuthenticationResult authenticationResult;

            if (string.IsNullOrEmpty(RefreshToken))
            {
                throw new UnauthorizedAccessException("No ha enviado un RefreshToken.");
            }

            authenticationResult = _authService.RefreshToken(RefreshToken);
            if (authenticationResult == null)
            {
                throw new UnauthorizedAccessException("No se realizó la autenticacion.");
            }

            return authenticationResult;
        }

        public void LogOutById(long id)
        {
            UsuarioModelo Usuario;
            if (id <= 0)
            {
                throw new ArgumentException("No se puede consultar el usuario porque el id no es válido.");
            }

            Usuario = _usuarioRepositorio.ListUsuarioPorId(id);
            if (Usuario.Id <= 0)
            {
                throw new UnauthorizedAccessException($"El id {id} no se encuentra asociado a un usuario.");
            }

            _authService.Logout(id);
        }

        //public void ActualizarUsuario(long id,UsuarioModelo UsuarioActualizar)
        //{
        //    UsuarioModelo UsuarioExistente;

        //    if (id <= 0)
        //    {
        //        throw new ArgumentException("El ID del usuario no´es válido.");
        //    }

        //    _userValidations.Validate(UsuarioActualizar);

        //    UsuarioExistente = _usuarioRepositorio.ListUsuarioPorId(id);
        //    if (UsuarioExistente == null || UsuarioExistente.Id <= 0)
        //    {
        //        throw new KeyNotFoundException("El Usuario que intenta actualizar, no se encuentra en el sistema.");
        //    }

        //    _usuarioRepositorio.ActualizarUsuario(id, UsuarioActualizar);
        //}
    }
}
