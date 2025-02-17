// DetalleLibro.tsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReviewList from '../components/ReviewList.tsx';
import ReviewForm from '../components/ReviewForm.tsx';
import { Libro } from '../Interfaces/Libro.ts';
import { Review } from '../Interfaces/Review.ts';
import { fetchWithAuth } from '../methods/fetchWithAuth.ts';
import { SessionExpiredError } from '../methods/SessionExpiredError.ts';
import { RequestOptions } from '../Interfaces/RequestOptions.ts';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { ResponseErrorGet } from '../methods/ResponseErrorGet.ts';
import { useAppContext } from '../components/AppContext.tsx';
import { PaginacionResponse } from '../Interfaces/PaginacionResponse.ts';

export default function DetalleLibro() {
  const context: AppContextType = useAppContext();
  const params = useParams();
  const idlibrostr = encodeURIComponent(`${params.idlibro}`);
  const idlibro = Number(idlibrostr);
  const navigate = useNavigate();

  const [book, setBook] = useState<Libro | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [reviewsLoading, setReviewsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  // Estado para la respuesta paginada
  const [paginatedReviews, setPaginatedReviews] = useState<PaginacionResponse<Review> | null>(null);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const itemsPerPage = 5;
  const groupedByBook = false;
  const showActions = true;

  const CtrlNameLibro = 'Libro';
  const CtrlNameReview = 'Review';

  const fetchLibroid = async (id: number) => {
    try {
      if (isNaN(idlibro) || !idlibro) {
        console.log('ID de Libro No válido');
        navigate('/');
        return;
      }

      let url = `${context.apiUrl}/${CtrlNameLibro}/ObtenerLibroPorid/${id}`;
      const response = await fetch(url);
      if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        setError(errorContent);
        return;
      }

      const data: Libro = await response.json();
      if (data.id <= 0) {
        console.error('El libro consultado no se encuentra registrado!', 'Error 3734334');
        navigate('/');
      }

      setBook(data);
    } catch (error) {
      setError(`Error 6436: ${error}`);
      console.error('Error fetching libro:', error);
    }
  };

  const fetchReviewslibro = async (libro: Libro) => {
    try {
      let url = `${context.apiUrl}/${CtrlNameReview}/ConsultarReviewsPorLibro`;
      const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(libro),
      });

      if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        setError(errorContent);
        return;
      }

      const data: Review[] = await response.json();
      setReviews(data);
      setReviewsLoading(false);
    } catch (error) {
      setError(`Error fetching reviews: ${error}`);
      console.error('Error fetching reviews:', error);
      setReviewsLoading(false);
    }
  };

  useEffect(() => {
    const fetchInitialData = async () => {
      if (!context.apiUrl) return; // Esperar a que apiUrl esté disponible
      try {
        await fetchLibroid(idlibro);
      } catch (error) {
        console.error('Error fetching data:', error);
        setError(`Error: ${error}`);
      }
    };

    fetchInitialData();
  }, [context.apiUrl, idlibro]);

  useEffect(() => {
    if (book) {
      setReviewsLoading(true);
      fetchReviewslibro(book);
    }
  }, [book, currentPage]);

  const handleReviewSubmit = async (newReviewdata: { rating: number; comment: string }) => {
    if (!context.token || !context.user) {
      console.error('No puede realizar una reseña sin estar logueado!', 'Error 21415');
      navigate('/Login');
      return;
    }

    const newReview: Review = {
      id: 0,
      calificacion: newReviewdata.rating,
      comentario: newReviewdata.comment,
      createdAt: new Date(),
      usuario: context.user,
      libro: book!,
    };

    try {
      const requestOptions: RequestOptions = {
        method: 'POST',
        body: newReview,
      };
      const response = await fetchWithAuth<void>(
        `${context.apiUrl}/${CtrlNameReview}/GuardarReview`,
        context.token,
        requestOptions,
        undefined,
        context
      );

      if (response.ok) {
        window.alert("Reseña agregada");
        await fetchReviewslibro(book!); // Refrescar las reseñas
      } else {
        const errorContent = await ResponseErrorGet(response);
        console.error('Error en la solicitud:', errorContent);
      }
    } catch (error) {
      if (error instanceof SessionExpiredError) {
        context.handleError(error);
      } else {
        console.error('Error 62362 durante envío de reseña:', error);
      }
    }
  };

  const handlePageChange = (newPage: number) => {
    if (paginatedReviews && newPage > 0 && newPage <= paginatedReviews.totalPaginas) {
      setCurrentPage(newPage);
    }
  };

  if (error) return <div>{error}</div>;
  if (!book) return <div>Cargando libro...</div>;

  return (
    <div className="max-w-4xl mx-auto p-4">
      {/* Sección de detalles del libro */}
      <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
        <h1 className="text-3xl font-bold mb-2">{book.title}</h1>
        <div className="flex gap-4 text-gray-600 mb-4">
          <p>Autor: {book.autor}</p>
          <p>Categoría: {book.categoria}</p>
        </div>
        <p className="text-gray-700 leading-relaxed">{book.resumen}</p>
      </div>

      {/* Sección de reseñas */}
      {context.user ? (
        <ReviewForm onSubmit={handleReviewSubmit} userEmail={context.user.correo} />
      ) : (
        <div className="bg-yellow-100 p-4 rounded-lg mb-6">
          <p>
            Debes{' '}
            <button onClick={() => navigate('/Login')} className="text-blue-500 hover:underline">
              iniciar sesión
            </button>{' '}
            para comentar
          </p>
        </div>
      )}

      {reviewsLoading ? (
        <div>Cargando reseñas...</div>
      ) : (
        <>
          <ReviewList reviews={reviews} 
          showActions = {showActions}
          groupedByBook = {groupedByBook}
          reviewsPerPage={itemsPerPage}  />
          {paginatedReviews && paginatedReviews.totalPaginas > 1 && (
            <div className="flex justify-center gap-2 mt-8">
              {Array.from({ length: paginatedReviews.totalPaginas }, (_, i) => (
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
        </>
      )}
    </div>
  );
}