import React from 'react';
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
}

const ReviewList = ({ reviews, showActions = false, groupedByBook = false }: ReviewListProps) => {
  const context = useAppContext() as AppContextType;

  const handleAction = (e: React.MouseEvent, callback: Function, id: number) => {
    e.stopPropagation();
    callback(id);
  };

  return (
    <div className="space-y-4">
      {reviews.map((review) => (
        <div key={review.id} className="p-4 bg-white rounded-lg shadow-md relative">
          {/* Encabezado de la reseña */}
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

          {/* Contenido */}
          <p className="text-gray-700">{review.comentario}</p>

          {/* Acciones */}
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
    </div>
  );
};

export default ReviewList;