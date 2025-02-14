using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Libros.Mappers;
using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Reviews.Mappers;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Dominio.Usuarios.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Reviews.Adaptadores
{
    public class ReviewRepositorioPostgreSQL : IReviewRepositorio
    {
        private readonly PostgreSQLDbContext _postgreSQLDbContext;
        private readonly MapToReviewEntity _mapToReviewEntity;
        private readonly MapToReviewDominioModel _mapToReviewDominioModel;

        public ReviewRepositorioPostgreSQL(PostgreSQLDbContext dbContext)
        {
            _postgreSQLDbContext = dbContext;
            _mapToReviewEntity = new MapToReviewEntity(dbContext);
            _mapToReviewDominioModel = new MapToReviewDominioModel();
        }
        public long AddReview(ReviewModel Review)
        {
            ReviewEntity reviewEntity = _mapToReviewEntity.MapToReviewEntidad(Review);
            _postgreSQLDbContext.Add(reviewEntity);
            SaveChanges();
            return reviewEntity.Id;
        }

        public ReviewModel ListReviewPorId(long id)
        {
            IQueryable<ReviewEntity> query = _postgreSQLDbContext.Reviews.Include(r => r.Usuario).Include(r=>r.Libro).ThenInclude(libro=>libro.Categoria).Where(l => l.Id == id);

            ReviewEntity reviewEntity = query.FirstOrDefault();

            if(reviewEntity == null)
            {
                return new ReviewModel();
            }

            ReviewModel review = _mapToReviewDominioModel.MapToReviewdomainmodel(reviewEntity);

            return review;
        }

        public List<ReviewModel> ListReviewPorUsuarioPaginado(UsuarioModelo Usuario, int skip, int tamanoPagina)
        {
            // Obtener los libros que tienen reseñas del usuario, paginando por libros
            var librosPaginados = _postgreSQLDbContext.Reviews
                .Where(r => r.UsuarioId == Usuario.Id)
                .Select(r => r.Libro) // Seleccionamos los libros asociados
                .Distinct() // Evitamos libros duplicados
                .OrderByDescending(libro => libro.Titulo) // Puedes cambiar el criterio de orden si lo necesitas
                .Skip(skip)
                .Take(tamanoPagina)
                .ToList();

            if (!librosPaginados.Any())
            {
                return new List<ReviewModel>();
            }

            // Obtener todas las reviews asociadas a los libros seleccionados
            List<ReviewEntity> reviews = _postgreSQLDbContext.Reviews.Include(r => r.Usuario).Include(r => r.Libro).ThenInclude(libro => libro.Categoria)
                .Where(r => librosPaginados.Select(l => l.Id).Contains(r.LibroId))
                .ToList();


            return _mapToReviewDominioModel.MapToReviewModeloList(reviews);
        }


        public List<ReviewModel> ListReviewPorLibro(LibroModelo Libro)
        {
            IQueryable<ReviewEntity> query = _postgreSQLDbContext.Reviews.Include(r => r.Usuario).Include(r => r.Libro).ThenInclude(libro=>libro.Categoria).Where(r => r.Libro.Id == Libro.Id).OrderByDescending(r=>r.CreatedAt);

            List<ReviewEntity> reviewEntityList = query.ToList();

            if (reviewEntityList == null)
            {
                return new List<ReviewModel>();
            }

            return _mapToReviewDominioModel.MapToReviewModeloList(reviewEntityList);
        }

        public int ConteoReviewsPorUsuario(UsuarioModelo Usuario)
        {
            IQueryable<ReviewEntity> query = _postgreSQLDbContext.Reviews.Where(r=>r.UsuarioId == Usuario.Id);

            return query.Count();
        }

        public int ConteoDistinctLibrosReviewsPorUsuario(UsuarioModelo Usuario)
        {
            int conteo = _postgreSQLDbContext.Reviews
             .Where(r => r.UsuarioId == Usuario.Id)
             .Select(r => r.LibroId)
             .Distinct()            
             .Count();

            return conteo;
        }

        public bool UpdateReviewParcial(ReviewModel review, Dictionary<string, object> cambios)
        {

            foreach (var cambio in cambios)
            {
                var propiedad = review.GetType().GetProperty(cambio.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                propiedad.SetValue(review, Convert.ChangeType(cambio.Value, propiedad.PropertyType));
            }
            
            _postgreSQLDbContext.SaveChanges();
            return true;
        }
        public bool DeleteReview(ReviewModel Review)
        {

            ReviewEntity reviewEntity = _mapToReviewEntity.MapToReviewEntidad(Review);
            _postgreSQLDbContext.Reviews.Remove(reviewEntity);

            SaveChanges();
            return true;
        }

        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }

    }
}
