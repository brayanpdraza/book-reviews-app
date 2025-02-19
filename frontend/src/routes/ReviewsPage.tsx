import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppContext } from '../components/AppContext.tsx';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import ReviewList from '../components/ReviewList.tsx';
import { Review } from '../Interfaces/Review.ts';
import { PaginacionResponse } from '../Interfaces/PaginacionResponse.ts';

interface GroupedReview {
  libro: { titulo: string; categoria: string };
  reviews: Review[];
}

export default function ReviewsPage() {
  const navigate = useNavigate();
  const context = useAppContext() as AppContextType;
  const ControllerName = 'Review';

  // Estado para la agrupación de reseñas
  const [groupedReviews, setGroupedReviews] = useState<Map<number, GroupedReview>>(new Map());
  // Estado de paginación
  const [currentPage, setCurrentPage] = useState<number>(1);
  const itemsPerPage = 4;
  const reviewsPerPage = 5;
  const [totalPages, setTotalPages] = useState<number>(0);

  const showActions = true;
  const groupedByBook = true;

  // Cada vez que cambie el token, el id del usuario o la página actual, se vuelve a consultar la data.
  useEffect(() => {
    if (!context.apiUrl) {
      return
    }

    fetchReviews();
  }, [context.apiUrl,currentPage, navigate]);

  const fetchReviews = async () => {
    try {
      if (!context.apiUrl) throw new Error("Error de configuración");
      
      // Llamada al endpoint paginado
      const url = `${context.apiUrl}/${ControllerName}/ConsultarReviewPorUsuarioPaginado/${currentPage}/${itemsPerPage} ?idUsuario=${context.user?.id}`;
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${context.token}`
        },
      });
      
      if (!response.ok) throw new Error('Error al obtener reseñas');
      
      // La respuesta tiene la forma de PaginacionResponse<Review>
      const data: PaginacionResponse<Review> = await response.json();
      setTotalPages(data.totalPaginas);
      groupReviews(data.items);
    } catch (error) {
      console.error("Error:", error);
      setGroupedReviews(new Map());
      return;
    }
    console.log('Reseñas obtenidas');
  };

  // Función que agrupa las reseñas por libro
  const groupReviews = (reviews: Review[]) => {
    const grouped = new Map<number, GroupedReview>();
    reviews.forEach(review => {
      const libroId = review.libro.id;
      if (!grouped.has(libroId)) {
        grouped.set(libroId, {
          libro: {
            titulo: review.libro.titulo,
            categoria: review.libro.categoria
          },
          reviews: []
        });
      }
      grouped.get(libroId)?.reviews.push(review);
    });
    setGroupedReviews(grouped);
  };

  const handlePageChange = (newPage: number) => {
    if (newPage > 0 && newPage <= totalPages) {
      setCurrentPage(newPage);
    }
  };

  const handleReviewUpdated = () => {
    fetchReviews();
  };

  const handleReviewDeleted = (id: number) => {
    setGroupedReviews(prev => {
      const newGroupedReviews = new Map<number,GroupedReview>(prev);
      newGroupedReviews.forEach(group => {
        group.reviews = group.reviews.filter(review => review.id !== id);
      });
      return newGroupedReviews;
    });
  };

  return (
    <div className="mainContainer p-4">
      <h1 className="text-2xl font-bold mb-6">Mis Reseñas</h1>
      
      {groupedReviews.size > 0 ? (
          (Array.from(groupedReviews.entries()) as [number, GroupedReview][]).map(([libroId, group]) => (
          <section key={libroId} className="mb-8 border-b pb-6">
            <div className="mb-4">
              <h2 className="text-xl font-semibold">{group.libro.titulo}</h2>
              <p className="text-sm text-gray-500">{group.libro.categoria}</p>
            </div>
            <ReviewList 
              reviews={group.reviews} 
              showActions={showActions} 
              groupedByBook={groupedByBook}
              reviewsPerPage={reviewsPerPage}
              onReviewUpdated={handleReviewUpdated}
              onReviewDeleted={handleReviewDeleted} 
            />
          </section>
        ))
      ) : (
        <p className="text-gray-500">No tienes reseñas registradas</p>
      )}
      
      {totalPages > 0 && (
        <div className="flex justify-center gap-2 mt-8">
          {Array.from({ length: totalPages }, (_, i) => (
            <button
              key={i + 1}
              onClick={() => handlePageChange(i + 1)}
              className={`px-4 py-2 border rounded-md ${
                currentPage === i + 1
                  ? "bg-blue-500 text-white border-blue-500"
                  : "bg-white text-gray-700 border-gray-300 hover:bg-gray-100"
              } focus:outline-none focus:ring-2 focus:ring-blue-500`}
            >
              {i + 1}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}