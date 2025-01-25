using AdaptadorPostgreSQL.CategoriasLibro.Entidades;
using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL
{
    public class PostgreSQLDbContext : DbContext
    {
        public PostgreSQLDbContext() { }
        public PostgreSQLDbContext(DbContextOptions<PostgreSQLDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=bookReviewApp;Username=brayan;Password=12345678;Pooling=true");
            }
        }

        public DbSet<ReviewEntity> Reviews { get; set; }
        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<LibroEntity> Libros { get; set; }
        public DbSet<CategoriasLibroEntity> CategoriasLibro { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //USUARIOS
            modelBuilder.Entity<UsuarioEntity>()
            .HasKey(u => u.Id); 

            modelBuilder.Entity<UsuarioEntity>()
            .Property(u => u.Correo)
            .IsRequired();

            modelBuilder.Entity<UsuarioEntity>()
            .Property(u => u.Password)
            .IsRequired();

            //LIBROS
            modelBuilder.Entity<LibroEntity>()
            .HasKey(u => u.Id);

            modelBuilder.Entity<LibroEntity>()
            .HasOne(r => r.Categoria)
            .WithMany(u => u.libroEntities)
            .HasForeignKey(r => r.CategoriasLibroId);

            //CategoriaLibro
            modelBuilder.Entity<CategoriasLibroEntity>()
            .HasKey(u => u.id);

            modelBuilder.Entity<CategoriasLibroEntity>()
            .Property(u => u.Nombre)
            .IsRequired();

            //REVIEWS
            modelBuilder.Entity<ReviewEntity>()
            .HasKey(u => u.Id);

            modelBuilder.Entity<ReviewEntity>()
            .Property(u => u.Calificacion)
            .IsRequired();

            modelBuilder.Entity<ReviewEntity>()
            .Property(u => u.Comentario)
            .IsRequired();


            modelBuilder.Entity<ReviewEntity>()
           .HasOne(r => r.Usuario)
           .WithMany(u => u.Reviews)
           .HasForeignKey(r => r.UsuarioId);

            modelBuilder.Entity<ReviewEntity>()
            .HasOne(r => r.Libro)
            .WithMany(l => l.Reviews)
            .HasForeignKey(r => r.LibroId);


        }
    }
}
