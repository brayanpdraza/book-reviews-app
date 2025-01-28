using AdaptadorPostgreSQL.Libros.Entidades;
using AdaptadorPostgreSQL.Libros.Mappers;
using AdaptadorPostgreSQL.Reviews.Entidades;
using AdaptadorPostgreSQL.Reviews.Mappers;
using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<ReviewModel> ListReviewPorLibro(LibroModelo Libro)
        {
            IQueryable<ReviewEntity> query = _postgreSQLDbContext.Reviews.Include(r => r.Usuario).Include(r => r.Libro).ThenInclude(libro=>libro.Categoria).Where(r => r.Libro.Id == Libro.Id).OrderByDescending(r=>r.CreatedAt);

            List<ReviewEntity> reviewEntityList = query.ToList();

            if (reviewEntityList == null)
            {
                return new List<ReviewModel>();
            }

            List<ReviewModel> reviewList = _mapToReviewDominioModel.MapToReviewModeloList(reviewEntityList);

            return reviewList;
        }

        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }
    }
}
