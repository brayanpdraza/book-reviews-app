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
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Determinar el entorno de ejecución
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// Cargar las variables desde el archivo .env solo en desarrollo
if (environment == "Development")
{
    Env.Load();
}

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")?.Trim();

if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL no está configurada.");
}

NpgsqlConnectionStringBuilder connectionBuilder;

if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
{
    try
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
            Password = userInfo[1].Replace("%40", "@"), // Desescapar '@'
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
    }
    catch (UriFormatException ex)
    {
        throw new Exception($"Formato inválido de DATABASE_URL: {ex.Message}");
    }
}
else
{
    // Si ya es una cadena de conexión estándar, úsala directamente
    connectionBuilder = new NpgsqlConnectionStringBuilder(databaseUrl);
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
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


builder.Services.AddScoped<UseCaseUsuario>();
builder.Services.AddScoped<UseCaseLibro>();
builder.Services.AddScoped<UseCaseReview>();
builder.Services.AddScoped<PostgreSQLDbContext>();
builder.Services.AddScoped<TokenValidationParameters>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthserviceJWT>();

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioPostgreSQL>();
builder.Services.AddScoped<ILibroRepositorio, LibroRepositorioPostgreSQL>();
builder.Services.AddScoped<IReviewRepositorio, ReviewRepositorioPostgreSQL>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenPostgreSQL>();

builder.Services.AddScoped<IEncription, PBKDF2Encription>();
builder.Services.AddScoped<IUserValidations, UserValidations>();
builder.Services.AddScoped<IReviewValidations, ReviewValidations>();

builder.Services.AddControllers();

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
