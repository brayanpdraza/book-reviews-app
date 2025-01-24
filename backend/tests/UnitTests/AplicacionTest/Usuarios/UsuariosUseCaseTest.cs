using Aplicacion.Usuarios;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Dominio.Usuarios.Servicios;
using Moq;

namespace AplicacionTest.Usuarios
{
    public class UsuariosUseCaseTest
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private readonly Mock<IEncription> _mockEncription;
        private readonly Mock<IUserValidations> _mockUserValidations;

        private readonly UseCaseUsuario _useCaseUsuario;
        private readonly UsuriosBuilderCaseTest usuriosBuilderCaseTest;

        public UsuariosUseCaseTest()
        {
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockEncription = new Mock<IEncription>();
            _mockUserValidations = new Mock<IUserValidations>();
            _useCaseUsuario = new UseCaseUsuario(
                _mockUsuarioRepositorio.Object,
                _mockEncription.Object,
                _mockUserValidations.Object
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
        public void ConsultarUsuarioCredenciales_CorreoVacio_LanzaExcepcion(string InvalidCorreo, string MessageError)
        {
            // Arrange

            string HashPassword = "hashedpass";
            string Password = "P4ssG@00d";

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.ConsultarUsuarioCredenciales(InvalidCorreo, Password));

            // Assert
            Assert.Equal(MessageError, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(InvalidCorreo), Times.Never);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(HashPassword, Password), Times.Never);
        }

        [Theory]
        [InlineData("", "Debe ingresar una Contraseņa.")]
        public void ConsultarUsuarioCredenciales_PasswordVacia_LanzaExcepcion(string invalidPassword, string MessageError)
        {
            // Arrange
            string Correo = "correo@prueba.com";
            string HashPassword = "hashedpass";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(Correo)).Returns((UsuarioModelo)null);

            //Act
            var exception = Assert.Throws<ArgumentException>(() => _useCaseUsuario.ConsultarUsuarioCredenciales(Correo, invalidPassword));

            // Assert
            Assert.Equal(MessageError, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(Correo), Times.Never);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(HashPassword, invalidPassword), Times.Never);
        }


        [Theory]
        [InlineData("correo@inexistente.com", "El correo ingresado no se encuentra Registrado.")]
        public void ConsultarUsuarioCredenciales_CorreoNoRegistrado_LanzaExcepcion(string InexistentCorreo, string MessageError)
        {
            // Arrange
            string HashPassword = "hashedpass";
            string Password = "P4ssG@00d";

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(InexistentCorreo)).Returns((UsuarioModelo)null);

            //Act
            var exception = Assert.Throws<Exception>(() => _useCaseUsuario.ConsultarUsuarioCredenciales(InexistentCorreo, Password));

            // Assert
            Assert.Equal(MessageError, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(InexistentCorreo), Times.Once);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(HashPassword, Password), Times.Never);
        }

        [Theory]
        [InlineData("Wr0ngP@sS", "La contraseņa proporcionada es Incorrecta.")]
        public void ConsultarUsuarioCredenciales_PassIncorrecta_LanzaExcepcion(string InvalidPass,string ErrorMessage)
        {
            // Arrange
            string HashedPass = "HashedPass"; 
            string Correo = "correo@prueba.com";

            UsuarioModelo usuarioConsultado = usuriosBuilderCaseTest.SetPassword(HashedPass).SetCorreo(Correo).Build();

            _mockUsuarioRepositorio.Setup(r => r.ListUsuarioPorCorreo(Correo)).Returns(usuarioConsultado);
            _mockEncription.Setup(r => r.VerificarClaveEncriptada(usuarioConsultado.Password, InvalidPass)).Returns(false);

            //Act
            var exception = Assert.Throws<Exception>(() => _useCaseUsuario.ConsultarUsuarioCredenciales(Correo, InvalidPass));

            // Assert
            Assert.Equal(ErrorMessage, exception.Message);


            _mockUsuarioRepositorio.Verify(r => r.ListUsuarioPorCorreo(Correo), Times.Once);
            _mockEncription.Verify(e => e.VerificarClaveEncriptada(usuarioConsultado.Password, InvalidPass), Times.Once);
        }
    }
}