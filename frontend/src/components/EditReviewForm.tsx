import React, { useState } from 'react';
import StarRating from './StarRating.tsx';
import { ReviewModifiers } from '../Interfaces/ReviewModifiers.ts';
import { useNavigationGuard } from '../methods/useNavigationGuard.ts';

interface EditReviewFormProps {
  initialRating: number;
  initialComment: string;
  onSubmit: (data: ReviewModifiers) => Promise<void>;
  onCancel: () => void;
}

const EditReviewForm = ({
  initialRating,
  initialComment,
  onSubmit,
  onCancel,
}: EditReviewFormProps) => {
  const [rating, setRating] = useState(initialRating);
  const [comment, setComment] = useState(initialComment);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const hasUnsavedChanges = comment !== initialComment || rating !== initialRating;
  const { confirmExit } = useNavigationGuard(hasUnsavedChanges);

  useNavigationGuard(hasUnsavedChanges);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

        const data: ReviewModifiers = {
            calificacion: rating,
            comentario: comment,
        };
  
        await onSubmit( data );
        setIsSubmitting(false); 
    };

  const handleCancel = () => {
      if (!confirmExit()) return; // Si el usuario cancela, no hace nada
      setRating(initialRating);
      setComment(initialComment);
      onCancel();
  }

  return (
    <div className="p-4 bg-white rounded-lg shadow-md mt-2">
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
          rows={3}
          required
        />
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={handleCancel}
            className="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50"
          >
            {isSubmitting ? 'Guardando...' : 'Guardar cambios'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default EditReviewForm;