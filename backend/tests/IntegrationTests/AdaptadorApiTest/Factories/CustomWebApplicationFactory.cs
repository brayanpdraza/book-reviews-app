using Microsoft.AspNetCore.Hosting;
using AdaptadorPostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AdaptadorAPITest.Data;
using DotNetEnv;
using Npgsql;

namespace AdaptadorAPITest.Factories
{
    internal class CustomWebApplicationFactory<Tprogram> : WebApplicationFactory<Tprogram> where Tprogram:class
    {
        //private readonly TestUserControllerDataSeed _dataSeed;

        //public CustomWebApplicationFactory(TestUserControllerDataSeed userControllerDataSeed)
        //{
        //    _dataSeed = userControllerDataSeed;
        //}

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 🔹 Buscar la carpeta raíz del proyecto subiendo varias carpetas
            var baseDir = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // 🟢 Cargar el .env ANTES de que la aplicación inicie
            if (environment == "Development")
            {
                var envPath = Path.Combine(projectRoot, ".env");
                if (!File.Exists(envPath))
                {
                    throw new Exception($"No se encontró el archivo de variables de entorno: {envPath}.");
                }
                Env.Load(envPath);
            }
            // 🟢 Obtener DATABASE_URL una sola vez
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URLTEST")?.Trim();
            if (string.IsNullOrEmpty(databaseUrl))
            {
                throw new Exception("DATABASE_URL no está configurada en las pruebas.");
            }

            // 🔹 Convertir la DATABASE_URL a una cadena de conexión válida
            NpgsqlConnectionStringBuilder connectionBuilder;
            try
            {
                if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
                {
                    var uri = new Uri(databaseUrl);
                    var userInfo = uri.UserInfo.Split(':');

                    if (userInfo.Length < 2)
                    {
                        throw new Exception("DATABASE_URL no contiene usuario y contraseña válidos.");
                    }

                    connectionBuilder = new NpgsqlConnectionStringBuilder
                    {
                        Host = uri.Host,
                        Port = uri.Port,
                        Database = uri.AbsolutePath.TrimStart('/'),
                        Username = userInfo[0],
                        Password = userInfo[1],
                        CommandTimeout = 300
                    };
                }
                else
                {
                    connectionBuilder = new NpgsqlConnectionStringBuilder(databaseUrl)
                    {
                        CommandTimeout = 300
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar DATABASE_URL: {ex.Message}");
            }

            Console.WriteLine($"🔍 Cadena de conexión usada en pruebas: {connectionBuilder}");

            // 🟢 Configurar la conexión en `ConfigureAppConfiguration`
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new[]
                {
                     new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", connectionBuilder.ToString())
                 });
            });

            // 🟢 Configurar servicios en `ConfigureServices`
            builder.ConfigureServices((context, services) =>
            {
                // ❌ Eliminar el DbContext original para evitar conflictos
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PostgreSQLDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // ✅ Agregar la nueva conexión con la BD de pruebas
                services.AddDbContext<PostgreSQLDbContext>(options =>
                    options.UseNpgsql(connectionBuilder.ToString()));

                // ✅ Agregar `TestUserControllerDataSeed`
                services.AddScoped<TestUserControllerDataSeed>();

                // 🟢 Ejecutar migraciones y sembrar datos en la BD de pruebas
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<PostgreSQLDbContext>();
                    var dataSeed = scope.ServiceProvider.GetRequiredService<TestUserControllerDataSeed>();

                    // 🛑 Asegurar que la BD anterior no existe
                    db.Database.EnsureDeleted();

                    // 🔄 Aplicar TODAS las migraciones pendientes
                    db.Database.EnsureCreated();  // <-- Verifica que las migraciones existen y están aplicadas correctamente

                    // 🔹 Sembrar datos iniciales (usuarios de prueba, etc.)
                    dataSeed.Seed(db);
                }
            });
        }
    }
}