import React, {useState } from 'react';
import StarRating from './StarRating.tsx';
import { useNavigationGuard } from '../methods/useNavigationGuard.ts';

interface ReviewFormProps {
  onSubmit: (review: { rating: number; comment: string }) => void;
  userEmail: string;
}

const ReviewForm = ({ onSubmit, userEmail }: ReviewFormProps) => {
  const initialRating = 5;
  const initialComment = "";
  const [rating, setRating] = useState(initialRating);
  const [comment, setComment] = useState(initialComment);
  const hasUnsavedChanges = comment !== initialComment || rating !== initialRating;

  const { confirmExit } = useNavigationGuard(hasUnsavedChanges);

  useNavigationGuard(hasUnsavedChanges);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit({ rating, comment });
    setComment('');
    setRating(5);
  };

  return (
    <div className="p-4 bg-white rounded-lg shadow-md">
      <h3 className="text-lg font-semibold mb-2">Escribe tu reseña como {userEmail}</h3>
      <form onSubmit={handleSubmit} className="space-y-4">
        <StarRating
          rating={rating}
          editable
          onRatingChange={setRating}
        />
        <textarea
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          className="w-full p-2 border rounded-md"
          placeholder="Escribe tu reseña..."
          rows={3}
          required
        />
        <button
          type="submit"
          className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition-colors"
        >
          Enviar reseña
        </button>
      </form>
    </div>
  );
};

export default ReviewForm;