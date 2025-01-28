import StarRating from './StarRating.tsx';
import React from 'react';
import {Review} from '../Interfaces/Review.ts'


interface ReviewListProps {
  reviews: Review[];
}

const ReviewList = ({ reviews }: ReviewListProps) => {
  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Reseñas</h2>
      {reviews.length === 0 ? (
        <p className="text-gray-500">Aún no hay reseñas para este libro</p>
      ) : (
        reviews.map((review) => (
          <div key={review.id} className="p-4 bg-white rounded-lg shadow-md">
            <div className="flex justify-between items-center mb-2">
              <div>
                <h4 className="font-semibold">{review.usuario.nombre}</h4>
                <p className="text-sm text-gray-500">{new Date(review.createdAt).toLocaleDateString()}</p>
              </div>
              <StarRating rating={review.calificacion} />
            </div>
            <p className="text-gray-700">{review.comentario}</p>
          </div>
        ))
      )}
    </div>
  );
};

export default ReviewList;