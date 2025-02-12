import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppContext } from '../components/AppContext.tsx';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import ReviewList from '../components/ReviewList.tsx';
import { Review } from '../Interfaces/Review.ts';

interface GroupedReview {
  libro: { titulo: string; categoria: string };
  reviews: Review[];
}

export default function ReviewsPage() {
  const navigate = useNavigate();
  const context = useAppContext() as AppContextType;
  const [groupedReviews, setGroupedReviews] = useState<Map<number, GroupedReview>>(new Map());
  const ShowActions = true;
  const groupedByBook = true;

  useEffect(() => {
    if (!context.isAuthenticated) navigate('/');
    else fetchReviews();
  }, [context.token, context.user?.id]);

  const fetchReviews = async () => {
    try {
      if (!context.apiUrl) throw new Error("Error de configuración");
      
      const response = await fetch(`${context.apiUrl}/Review/ConsultarReviewPorUsuario`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${context.token}`
        },
        body: JSON.stringify(context.user)
      });

      if (!response.ok) throw new Error('Error al obtener reseñas');
      
      const data: Review[] = await response.json();
      groupReviews(data);
    } catch (error) {
      console.error("Error:", error);
      setGroupedReviews(new Map());
    }
  };

  const groupReviews = (reviews: Review[]) => {
    const grouped = new Map<number, GroupedReview>();
    
    reviews.forEach(review => {
      const libroId = review.libro.id;
      if (!grouped.has(libroId)) {
        grouped.set(libroId, {
          libro: review.libro,
          reviews: []
        });
      }
      grouped.get(libroId)?.reviews.push(review);
    });
    
    setGroupedReviews(grouped);
  };

  return (
    <div className="mainContainer p-4">
      <h1 className="text-2xl font-bold mb-6">Mis Reseñas</h1>
      
      {Array.from(groupedReviews.entries()).map(([libroId, grouped]) => (
        <section key={libroId} className="mb-8 border-b pb-6">
          <div className="mb-4">
            <h2 className="text-xl font-semibold">{grouped.libro.titulo}</h2>
            <p className="text-sm text-gray-500">{grouped.libro.categoria}</p>
          </div>
          
          <ReviewList 
            reviews={grouped.reviews} 
            showActions 
            groupedByBook
          />
        </section>
      ))}
      
      {groupedReviews.size === 0 && (
        <p className="text-gray-500">No tienes reseñas registradas</p>
      )}
    </div>
  );
}