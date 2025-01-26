using AdaptadorAPI.Contratos;
using AdaptadorAPI.Implementaciones;
using AdaptadorEncripter;
using AdaptadorPostgreSQL;
using AdaptadorPostgreSQL.Libros.Adaptadores;
using AdaptadorPostgreSQL.Libros.Mappers;
using AdaptadorPostgreSQL.Reviews.Adaptadores;
using AdaptadorPostgreSQL.Usuarios.Adaptadores;
using AdaptadorPostgreSQL.Usuarios.Mappers;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
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

app.Run();
