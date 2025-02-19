import React, { useState,useEffect } from 'react';
import { Review } from '../Interfaces/Review.ts';
import StarRating from './StarRating.tsx';
import { useAppContext } from '../components/AppContext.tsx';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { ReviewModifiers } from '../Interfaces/ReviewModifiers.ts';
import { SessionExpiredError } from '../methods/SessionExpiredError.ts';
import { EditReview } from '../methods/EditReview.ts';
import { DeleteReview } from '../methods/DeleteReview.ts';
import EditReviewForm from './EditReviewForm.tsx';

interface ReviewListProps {
  reviews: Review[];
  showActions?: boolean;
  groupedByBook?: boolean;
  reviewsPerPage?: number;
  onReviewUpdated?: () => void;
  onReviewDeleted?: (id: number) => void;
}

const ReviewList = ({ reviews, showActions = false, groupedByBook = false, reviewsPerPage = 10, onReviewUpdated, onReviewDeleted }: ReviewListProps) => {
  const context = useAppContext() as AppContextType;
  const [currentPage, setCurrentPage] = useState(1);
  const [editingReviewId, setEditingReviewId] = useState<number | null>(null);
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

  const handleDeleteClick = async (e: React.MouseEvent, callback: Function, id: number) => {
    e.stopPropagation();

    const confirmDelete = window.confirm(
      editingReviewId === id
        ? '¿Estás seguro de que quieres eliminar esta reseña? Perderás los cambios no guardados.'
        : '¿Estás seguro de que quieres eliminar esta reseña?'
    );
  
    if (!confirmDelete) {
      return; // No hacer nada si el usuario cancela
    }  
    try {
      await DeleteReview(id, context);
      onReviewDeleted?.(id); // ✅ Notificar que una reseña fue eliminada
    } 
    catch (error) 
    {
      if (error instanceof SessionExpiredError) {
        context.handleError(error);
      } else {
        console.error('Error 87678 durante Eliminación de reseña:', error);
      }
      return
    }

    if (editingReviewId === id) {
      setEditingReviewId(null);
    }
      
  };

  const handleEditClick = (e: React.MouseEvent, reviewId: number) => {
    e.stopPropagation();
  
    if (editingReviewId !== null && editingReviewId !== reviewId) {
      const confirmChange = window.confirm(
        'Tienes cambios sin guardar en otra reseña. ¿Estás seguro de que quieres descartarlos?'
      );
  
      if (!confirmChange) {
        return; // No hacer nada si el usuario cancela
      }

    }
    setEditingReviewId(reviewId); // Activar la edición de la nueva reseña
  };

  const EditarReseña = async (id: number, data: ReviewModifiers) => 
  {
    try 
    {
      await EditReview(id, data,context); 
      onReviewUpdated?.(); // Notificar que se editó una reseña
    } 
    catch (error) 
    {
      if (error instanceof SessionExpiredError) {
        context.handleError(error);
      } else {
        console.error('Error 78972 durante Edición de reseña:', error);
      }
      return;
    }
    setEditingReviewId(null); // Salir del modo edición

  }

  const handlePageChange = (newPage: number) => {
    if (newPage > 0 && newPage <= totalPages) {
      setCurrentPage(newPage);
    }
  };


  return (
    <div className="space-y-4">
      {selectedReviews.map((review) => (
        <div key={review.id} className="p-4 bg-white rounded-lg shadow-md relative">
        {/* Contenedor principal con padding derecho para los botones */}
        <div className="pr-8"> 
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
            
            {/* Contenedor de estrellas con ancho mínimo */}
            <div className="min-w-[100px]">
              <StarRating rating={review.calificacion} />
            </div>
          </div>
      
          <p className="text-gray-700">{review.comentario}</p>
        </div>
      
        {showActions && context.user?.id === review.usuario.id && (
          <div className="absolute top-4 right-4 flex flex-col gap-2">
            <button
               onClick={(e) => handleEditClick(e, review.id)}
              className={`text-blue-600 hover:text-blue-800 ${
                editingReviewId !== null && editingReviewId !== review.id ? 'opacity-50 cursor-not-allowed' : ''
              }`}
              disabled={editingReviewId !== null && editingReviewId !== review.id}
            >
              ✏️
            </button>
            <button
              onClick={(e) => handleDeleteClick(e, DeleteReview, review.id)}
              
              className="text-red-600 hover:text-red-800"
            >
              🗑️
            </button>
          </div>
        )}

          {/* Formulario de edición */}
          {editingReviewId === review.id && (
            <EditReviewForm
            initialRating={review.calificacion}
            initialComment={review.comentario}
            onSubmit={async (data: ReviewModifiers) => {
              await EditarReseña(review.id, data);
            }}
            onCancel={() => setEditingReviewId(null)}
          />
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
