using Aplicacion.Usuarios;
using Dominio.Entidades.Usuarios.Modelo;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Reviews.Modelo;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Moq;

namespace AplicacionTest.Usuarios
{
    public class UsuariosUseCaseTest
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private readonly Mock<IEncription> _mockEncription;
        private readonly Mock<IUserValidations> _mockUserValidations;
        private readonly Mock<IAuthService> _mockAuthService;

        private readonly UseCaseUsuario _useCaseUsuario;
        private readonly UsuriosBuilderCaseTest usuriosBuilderCaseTest;

        public UsuariosUseCaseTest()
        {
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockEncription = new Mock<IEncription>();
            _mockUserValidations = new Mock<IUserValidations>();
            _mockAuthService = new Mock<IAuthService>();

            _useCaseUsuario = new UseCaseUsuario(
                _mockUsuarioRepositorio.Object,
                _mockEncription.Object,
                _mockUserValidations.Object,
                _mockAuthService.Object
            );
            usuriosBuilderCaseTest = new UsuriosBuilderCaseTest(_mockUserValidations.Object);
        }

        [Fact]
        public void AddUsuario_UsuarioYaRegistrado_LanzaExcepcion()
        {
            // Arrange
            UsuarioModelo usuario = usuriosBuilderCaseTest.Build();
            UsuarioModelo usuarioRegistrado = usuriosBuilderCaseTest.Build();

            string Password = usuario.Password;
            string ErrorMessage= "El correo ingresado ya se encuentra Registrado en el sistema.";

            _mockUserValidations.Setup(u => u.Validate(usuario)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(usuario.Correo)).Returns(usuarioRegistrado);

            // Act Assert
            var exception = Assert.Throws<Exception>(() => _useCaseUsuario.AddUsuario(usuario));
            Assert.Equal(ErrorMessage, exception.Message);


            _mockUserValidations.Verify(r => r.Validate(usuario), Times.Once);
            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(usuario.Correo), Times.Once);
            _mockEncription.Verify(e => e.Encriptar(Password), Times.Never);
            _mockUsuarioRepositorio.Verify(r => r.AddUsuario(usuario), Times.Never);
        }

        [Fact]
        public void AddUsuario_UsuarioNoCreado_LanzaExcepcion()
        {
            // Arrange
            UsuarioModelo usuario = usuriosBuilderCaseTest.Build();

            string Password = usuario.Password;

            _mockUserValidations.Setup(u => u.Validate(usuario)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(usuario.Correo)).Returns((UsuarioModelo)null);
            _mockEncription.Setup(e => e.Encriptar(Password)).Returns("encryptedPassword");
            _mockUsuarioRepositorio.Setup(r => r.AddUsuario(usuario)).Returns(0);

            // Act
            var exception = Assert.Throws<Exception>(() => _useCaseUsuario.AddUsuario(usuario));

            // Assert
            Assert.Equal("El usuario no ha sido creado.", exception.Message);

            _mockUserValidations.Verify(r => r.Validate(usuario), Times.Once);
            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(usuario.Correo), Times.Once);
            _mockEncription.Verify(e => e.Encriptar(Password), Times.Once);
            _mockUsuarioRepositorio.Verify(r => r.AddUsuario(usuario), Times.Once);
        }

        [Fact]
        public void AddUsuario_UsuarioValido_RetornaId()
        {
            // Arrange
            UsuarioModelo usuario = usuriosBuilderCaseTest.Build();
            
            long idCreado;
            string Password = usuario.Password;

            _mockUserValidations.Setup(v => v.Validate(usuario)).Verifiable();
            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(usuario.Correo)).Returns((UsuarioModelo)null);
            _mockEncription.Setup(e => e.Encriptar(Password)).Returns("encryptedPassword");
            _mockUsuarioRepositorio.Setup(r => r.AddUsuario(usuario)).Returns(1);

            // Act
            idCreado = _useCaseUsuario.AddUsuario(usuario);

            // Assert
            Assert.Equal(1, idCreado);
            _mockUserValidations.Verify(v => v.Validate(usuario), Times.Once);
            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(usuario.Correo), Times.Once);
            _mockEncription.Verify(e => e.Encriptar(Password), Times.Once);
            _mockUsuarioRepositorio.Verify(r => r.AddUsuario(usuario), Times.Once);
        }

        [Theory]
        [InlineData("", "Debe ingresar un Correo.")]
        public void AutenticacionByCredenciales_CorreoVacio_LanzaExcepcion(string InvalidCorreo, string MessageError)
        {
            // Arrange
            UsuarioModelo usuario = new UsuarioModelo();
            string HashPassword = "hashedpass";
            string Password = "P4ssG@00d";

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.AutenticacionByCredenciales(InvalidCorreo, Password));

            // Assert
            Assert.Equal(MessageError, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(InvalidCorreo), Times.Never);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(Password, HashPassword), Times.Never);
            _mockAuthService.Verify(a => a.Authenticate(usuario), Times.Never);
        }

        [Theory]
        [InlineData("", "Debe ingresar una Contraseña.")]
        public void AutenticacionByCredenciales_PasswordVacia_LanzaExcepcion(string invalidPassword, string MessageError)
        {
            // Arrange
            UsuarioModelo usuario = null;
            string Correo = "correo@prueba.com";
            string HashPassword = "hashedpass";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(Correo)).Returns((UsuarioModelo)null);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.AutenticacionByCredenciales(Correo, invalidPassword));

            // Assert
            Assert.Equal(MessageError, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(Correo), Times.Never);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(invalidPassword, HashPassword), Times.Never);
            _mockAuthService.Verify(a => a.Authenticate(usuario), Times.Never);
        }

        [Theory]
        [InlineData("correo@inexistente.com", "El correo ingresado no se encuentra Registrado.")]
        public void AutenticacionByCredenciales_CorreoNoRegistrado_LanzaExcepcion(string InexistentCorreo, string MessageError)
        {
            // Arrange
            UsuarioModelo usuario = null;
            string HashPassword = "hashedpass";
            string Password = "P4ssG@00d";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(InexistentCorreo)).Returns((UsuarioModelo)null);

            //Act
            var exception = Assert.Throws<KeyNotFoundException>(() => _useCaseUsuario.AutenticacionByCredenciales(InexistentCorreo, Password));

            // Assert
            Assert.Equal(MessageError, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(InexistentCorreo), Times.Once);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(Password, HashPassword), Times.Never);
            _mockAuthService.Verify(a => a.Authenticate(usuario), Times.Never);
        }

        [Theory]
        [InlineData("Wr0ngP@sS", "La contraseña proporcionada es Incorrecta.")]
        public void AutenticacionByCredenciales_PassIncorrecta_LanzaExcepcion(string InvalidPass,string ErrorMessage)
        {
            // Arrange
            UsuarioModelo usuario;
            string HashedPass = "HashedPass"; 
            string Correo = "correo@prueba.com";

            UsuarioModelo usuarioConsultado = usuriosBuilderCaseTest.SetPassword(HashedPass).SetCorreo(Correo).Build();
            usuario = usuarioConsultado;

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(Correo)).Returns(usuarioConsultado);
            _mockEncription.Setup(r => r.VerificarClaveEncriptada(InvalidPass, HashedPass)).Returns(false);

            //Act
            var exception = Assert.Throws<UnauthorizedAccessException>(() => _useCaseUsuario.AutenticacionByCredenciales(Correo, InvalidPass));

            // Assert
            Assert.Equal(ErrorMessage, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(Correo), Times.Once);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(InvalidPass, usuarioConsultado.Password), Times.Once);
            _mockAuthService.Verify(a => a.Authenticate(usuario), Times.Never);
        }

        [Fact]
        public void AutenticacionByCredenciales_AutenticacionIncorrecta_LanzaExcepcion()
        {
            // Arrange}
            AuthenticationResult Result = null;
            UsuarioModelo usuario;
            string HashedPass = "HashedPass";
            string Pass = "P4ssG@0d";
            string Correo = "correo@prueba.com";
            string ErrorMessage = "No se realizó la autenticacion.";

            UsuarioModelo usuarioConsultado = usuriosBuilderCaseTest.SetPassword(HashedPass).SetCorreo(Correo).Build();
            usuario = usuarioConsultado;

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(Correo)).Returns(usuarioConsultado);
            _mockEncription.Setup(r => r.VerificarClaveEncriptada(Pass, HashedPass)).Returns(true);
            _mockAuthService.Setup(r => r.Authenticate(usuario)).Returns(Result);

            //Act
            var exception = Assert.Throws<UnauthorizedAccessException>(() => _useCaseUsuario.AutenticacionByCredenciales(Correo, Pass));

            // Assert
            Assert.Equal(ErrorMessage, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(Correo), Times.Once);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(Pass, usuarioConsultado.Password), Times.Once);
            _mockAuthService.Verify(a => a.Authenticate(usuario), Times.Once);
        }


        [Fact]
        public void LogoutByCredenciales_AutenticacionIncorrecta_LanzaExcepcion()
        {
            // Arrange}
            LogoutRequest Result = null;
            string AccessToken = null;
            string ErrorMessage = "El access token no puede estar vacío.";
            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.LogOutByAccessToken(AccessToken));

            // Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockAuthService.Verify(a => a.Logout(AccessToken), Times.Never);
        }


        [Theory]
        [InlineData(0, "No se puede consultar el usuario porque el id no es válido.")]
        public void ConsultarUsuarioPoriD_idErrores_LanzaExcepcion(long id, string ErrorMessage)
        {
            // Arrange


            // Act 
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.ConsultarUsuarioPorId(id));

            //Assert
            Assert.Equal(ErrorMessage, exception.Message);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(id), Times.Never);
        }


        [Fact]
        public void ConsultarUsuarioporiD_ReseñasExistentes_ReturnsReview()
        {
            // Arrange
            UsuarioModelo Usuario = usuriosBuilderCaseTest.Build();
            long idExistente = 1;

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorId(idExistente)).Returns(Usuario);

            // Act 
            UsuarioModelo UsuarioResultado = _useCaseUsuario.ConsultarUsuarioPorId(idExistente);

            //Assert
            Assert.Equal(Usuario, UsuarioResultado);

            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorId(idExistente), Times.Once);
        }
    }
}