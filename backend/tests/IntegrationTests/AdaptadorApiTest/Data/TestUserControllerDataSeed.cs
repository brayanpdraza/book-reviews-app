using Microsoft.EntityFrameworkCore;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL;
using Dominio;
using AdaptadorAPI;
using Dominio.Servicios.ServicioEncripcion.Contratos;

namespace AdaptadorAPITest.Data
{
    internal class TestUserControllerDataSeed
    {
        private readonly IEncription _encription;

        public TestUserControllerDataSeed (IEncription encription)
        {
            _encription = encription;
        }

        public void Seed(PostgreSQLDbContext context)
        {
            string pass = "P4ss123@";
            string passEncript = _encription.Encriptar(pass);
            context.Usuarios.Add(new UsuarioEntity
            {
                Id = 1,
                FotoPerfil = null,
                Nombre = "Usuario Pruebas Locales",
                Correo = "test@example.com",
                Password = passEncript,
            });

            context.SaveChanges();
        }
    }
}
