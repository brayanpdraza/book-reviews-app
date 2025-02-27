using AdaptadorPostgreSQL;
using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.CategoriasLibro.Entidades;
using Dominio.Servicios.ServicioEncripcion.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorApiTest.Data
{
    public class TestLibroControllerDataSeed
    {
        private readonly CategoriasLibroEntity categoria_1;
        private readonly CategoriasLibroEntity categoria_2;
        private readonly CategoriasLibroEntity categoria_3;

        public TestLibroControllerDataSeed()
        {
            categoria_1 = new CategoriasLibroEntity
            {
                Nombre = "MISTERIO",
                Descripcion = "Libros de misterio",
            };
            categoria_2 = new CategoriasLibroEntity
            {
                Nombre = "ROMANCE",
                Descripcion = "Libros románticos",
            };
            categoria_3 = new CategoriasLibroEntity
            {
                Nombre = "TERROR",
                Descripcion = "Libros Terroríficos",
            };
        }

        public void SeedCategoriasLibro(PostgreSQLDbContext context)
        {

            context.CategoriasLibro.Add(categoria_1);

            context.CategoriasLibro.Add(categoria_2);

            context.CategoriasLibro.Add(categoria_3);

            context.SaveChanges();
        }

        public void SeedLibros(PostgreSQLDbContext context)
        {

            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro de misterio 1",
                Autor = "Autor 1",
                CategoriasLibroId = categoria_1.id,
                Categoria = categoria_1,
                Resumen = "Libro de misterio 1 muy misterioso"
            });
            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro de misterio 2",
                Autor = "Autor 2",
                CategoriasLibroId = categoria_1.id,
                Categoria = categoria_1,
                Resumen = "Libro de misterio 2 la venganza"
            });

            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro Romántico 1",
                Autor = "Autor 3",
                CategoriasLibroId = categoria_2.id,
                Categoria = categoria_2,
                Resumen = "Libro de romance 1 muy romántico"
            });
            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro Romántico 2",
                Autor = "Autor 4",
                CategoriasLibroId = categoria_2.id,
                Categoria = categoria_2,
                Resumen = "Libro de romance 2 muy romántico ahora es personal"
            });
            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro Romántico 3",
                Autor = "Autor 5",
                CategoriasLibroId = categoria_2.id,
                Categoria = categoria_2,
                Resumen = "Libro de romance 3 muy romántico revenge"
            });

            context.Libros.Add(new LibroEntity
            {
                Titulo = "Libro Terror 1",
                Autor = "Autor 6",
                CategoriasLibroId = categoria_3.id,
                Categoria = categoria_3,
                Resumen = "Libro de Terror 1 muy Terrorífico"
            });

            context.SaveChanges();
        }
    }
}
