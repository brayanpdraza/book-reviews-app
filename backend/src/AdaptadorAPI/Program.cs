using AdaptadorAPI.Contratos;
using AdaptadorAPI.Implementaciones;
using AdaptadorEncripter;
using AdaptadorPostgreSQL;
using AdaptadorPostgreSQL.Libros.Adaptadores;
using AdaptadorPostgreSQL.Reviews.Adaptadores;
using AdaptadorPostgreSQL.Usuarios.Adaptadores;
using Aplicacion.Libros;
using Aplicacion.Reviews;
using Aplicacion.Usuarios;
using Aplicacion.Methods;
using Dominio.Entidades.Libros.Puertos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Entidades.Usuarios.Puertos;
using Dominio.Reviews.Servicios;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using Dominio.Usuarios.Puertos;
using Dominio.Usuarios.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dominio.Entidades.Reviews.Servicios;
using Dominio.Servicios.ServicioValidaciones.Contratos;
using Dominio.Servicios.ServicioValidaciones.Implementaciones;
using AdaptadorAPI.Servicios.Contratos;
using AdaptadorAPI.Servicios.Implementaciones;
using AdaptadorAPI.Servicios;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

[assembly: InternalsVisibleTo("AdaptadorAPITest")]

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Determinar el entorno de ejecución

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

// Cargar las variables desde el archivo .env solo en desarrollo y test

if (environment == "Development")
{
    Env.Load();
}

var databaseUrl = "";

if (environment == "Test")
{
    databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URLTEST")?.Trim();
}
else
{
    databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")?.Trim();
}


if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL no está configurada.");
}

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
        };
    }
    else
    {
        connectionBuilder = new NpgsqlConnectionStringBuilder(databaseUrl);
    }

    // Establecer propiedades comunes
    //connectionBuilder.SslMode = SslMode.Require;
    //connectionBuilder.TrustServerCertificate = true;
    connectionBuilder.CommandTimeout = 300;
}
catch (UriFormatException ex)
{
    throw new Exception($"Formato inválido de DATABASE_URL: {ex.Message}");
}
var connectionString = connectionBuilder.ToString();
Console.WriteLine($"🔗 Conexión final: {connectionString}"); // Debug

builder.Services.AddDbContext<PostgreSQLDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tu API", Version = "v1" });

    // Configuración para marcar parámetros opcionales correctamente
    c.DescribeAllParametersInCamelCase();
    c.SupportNonNullableReferenceTypes();
});

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:3000")  // Origen permitido
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();  // Si necesitas enviar credenciales (cookies, tokens)
    });
});

// Configurar JWT
var secretKey = Environment.GetEnvironmentVariable("Jwt_SecretKey");
var issuer = Environment.GetEnvironmentVariable("Jwt_Issuer");
var audience = Environment.GetEnvironmentVariable("Jwt_Audience");
var AccessExpiry = Environment.GetEnvironmentVariable("Jwt_AccessTokenExpiration");
var RefreshExpiry = Environment.GetEnvironmentVariable("Jwt_RefreshTokenExpiration");


if (string.IsNullOrEmpty(secretKey))
    throw new Exception("La clave secreta JWT no está configurada en las variables de entorno.");
if (string.IsNullOrEmpty(issuer))
    throw new Exception("El emisor JWT no está configurado en las variables de entorno.");
if (string.IsNullOrEmpty(audience))
    throw new Exception("La audiencia JWT no está configurada en las variables de entorno.");
if (string.IsNullOrEmpty(AccessExpiry))
    throw new Exception("El tiempo de Expiración del token no está configurado en las variables de entorno.");
if (string.IsNullOrEmpty(RefreshExpiry))
    throw new Exception("El tiempo de Expiración del token de refresco no está configurada en las variables de entorno.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    };
})

.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict; 
});

builder.Services.AddAuthorization();


// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/mi-app.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<MetodosAuxiliares>();
builder.Services.AddScoped<UseCaseUsuario>();
builder.Services.AddScoped<UseCaseLibro>();
builder.Services.AddScoped<UseCaseReview>();
builder.Services.AddScoped<PostgreSQLDbContext>();
builder.Services.AddScoped<TokenValidationParameters>();
builder.Services.AddScoped<ObtenerDatosUsuarioToken>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthserviceJWT>();

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioPostgreSQL>();
builder.Services.AddScoped<ILibroRepositorio, LibroRepositorioPostgreSQL>();
builder.Services.AddScoped<IReviewRepositorio, ReviewRepositorioPostgreSQL>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenPostgreSQL>();

builder.Services.AddScoped<IEncription, PBKDF2Encription>();
builder.Services.AddScoped<IUserValidations, UserValidations>();
builder.Services.AddScoped<IReviewValidations, ReviewValidations>();
builder.Services.AddScoped<IReviewPartialUpdateValidations, ReviewPartialUpdateValidations>();
builder.Services.AddScoped<IpropertyModelValidate, PropertyValidator>();

builder.Services.AddScoped<IconverterJsonElementToDictionary, ConvertirJsonElementToDiccionarioTextJson>();

builder.Services.AddControllers();

//PARA INTEGRATION TESTS



var app = builder.Build();

app.UseAuthentication(); 
app.UseAuthorization(); 
//app.UseMiddleware<JwtMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowFrontend");

app.Run();

