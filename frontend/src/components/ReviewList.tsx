import React, { useState,useEffect } from 'react';
import { Review } from '../Interfaces/Review.ts';
import StarRating from './StarRating.tsx';
import { useAppContext } from '../components/AppContext.tsx';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { EditReview } from '../methods/EditReview.ts';
import { DeleteReview } from '../methods/DeleteReview.ts';

interface ReviewListProps {
  reviews: Review[];
  showActions?: boolean;
  groupedByBook?: boolean;
  reviewsPerPage?: number;
}

const ReviewList = ({ reviews, showActions = false, groupedByBook = false, reviewsPerPage = 10 }: ReviewListProps) => {
  const context = useAppContext() as AppContextType;
  const [currentPage, setCurrentPage] = useState(1);
  const totalPages = Math.ceil(reviews.length / reviewsPerPage);
  let startIndex = (currentPage - 1) * reviewsPerPage;
  let selectedReviews = reviews.slice(startIndex, startIndex + reviewsPerPage);

  // Cargar libros al cambiar de página
  useEffect(() => {
    if (!context.apiUrl) {
      return
    }
    manageReviews();
  }, [context.apiUrl,currentPage]);

  const manageReviews = () => {
    startIndex = (currentPage - 1) * reviewsPerPage;
    selectedReviews = reviews.slice(startIndex, startIndex + reviewsPerPage);
  }

  const handleAction = (e: React.MouseEvent, callback: Function, id: number) => {
    e.stopPropagation();
    callback(id);
  };

  const handlePageChange = (newPage: number) => {
    if (newPage > 0 && newPage <= totalPages) {
      setCurrentPage(newPage);
    }
  };


  return (
    <div className="space-y-4">
      {selectedReviews.map((review) => (
        <div key={review.id} className="p-4 bg-white rounded-lg shadow-md relative">
          <div className="flex justify-between items-center mb-2">
            <div>
              <h4 className="font-semibold">
                {context.user?.id === review.usuario.id ? 'Yo' : review.usuario.nombre}
              </h4>
              {groupedByBook && (
                <p className="text-sm text-gray-500">
                  {new Date(review.createdAt).toLocaleDateString()}
                </p>
              )}
            </div>
            <StarRating rating={review.calificacion} />
          </div>

          <p className="text-gray-700">{review.comentario}</p>

          {showActions && context.user?.id === review.usuario.id && (
            <div className="absolute top-2 right-2 flex gap-2">
              <button
                onClick={(e) => handleAction(e, EditReview, review.id)}
                className="text-blue-600 hover:text-blue-800"
              >
                ✏️
              </button>
              <button
                onClick={(e) => handleAction(e, DeleteReview, review.id)}
                className="text-red-600 hover:text-red-800"
              >
                🗑️
              </button>
            </div>
          )}
        </div>
      ))}

      {/* Paginación */}
      {totalPages > 1 && (
        <div className="flex justify-center space-x-2 mt-4">
          <button
            onClick={() => handlePageChange(currentPage - 1)}
            disabled={currentPage === 1}
            className={`px-3 py-1 rounded ${currentPage === 1 ? 'bg-gray-300' : 'bg-blue-500 text-white'}`}
          >
            Anterior
          </button>
          <span className="px-3 py-1">{currentPage} / {totalPages}</span>
          <button
            onClick={() => handlePageChange(currentPage + 1)}
            disabled={currentPage === totalPages}
            className={`px-3 py-1 rounded ${currentPage === totalPages ? 'bg-gray-300' : 'bg-blue-500 text-white'}`}
          >
            Siguiente
          </button>
        </div>
      )}
    </div>
  );
};

export default ReviewList;
