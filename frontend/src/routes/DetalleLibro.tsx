import React, { useState, useEffect } from 'react';
import { useParams, useNavigate,useLocation } from 'react-router-dom';
import ReviewList from '../components/ReviewList.tsx';
import ReviewForm from '../components/ReviewForm.tsx';
import { jwtDecode } from 'jwt-decode';
import {Libro} from '../Interfaces/Libro.ts'
import {Review} from '../Interfaces/Review.ts'
import {Usuario} from '../Interfaces/Usuario.ts'
import {fetchWithAuth} from '../methods/fetchWithAuth.ts'
import {fetchConfig} from '../methods/fetchConfig.ts'
import { GetAccessToken } from '../methods/GetAccessToken.ts';
import {SessionExpiredError} from '../methods/SessionExpiredError.ts';
import {RequestOptions} from '../Interfaces/RequestOptions.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';

interface JwtPayload {
    id : number;
    correo: string;
    nombre: string;
  }
  
export default function DetalleLibro(){  
const params = useParams();
const idlibrostr = encodeURIComponent(`${params.idlibro}`);
const idlibro = Number(idlibrostr);
 const location = useLocation();

const navigate = useNavigate();
const [book, setBook] = useState<Libro | null>(null);
const [reviews, setReviews] = useState<Review[]>([]);
const [userEmail, setUserEmail] = useState<string | null>(null);
const [userNombre, setUserNombre] = useState<string | null>(null);
const [userId, setUserId] = useState<number | null>(0);
const [loading, setLoading] = useState(true);
const [reviewsLoading, setReviewsLoading] = useState(true);
 const [apiUrl, setAPIUrl] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loadingConfig, setLoadingConfig] = useState(true);
  const CtrlNameLibro = 'Libro';
  const CtrlNameReview = 'Review';

  const fetchLibroid = async (id:number) => {
    try {

        if (isNaN(idlibro) || !idlibro) {
            console.log('ID de Libro No válido')
            navigate('/')
            return;
          }
        
      let url = `${apiUrl}/${CtrlNameLibro}/ObtenerLibroPorid/${id}`;

      const response = await fetch(url);
      if (!response.ok) { 
        const errorContent = await ResponseErrorGet(response);
        setError(errorContent);  
        
        return;
      }
  
      const data: Libro = await response.json();

      if(data.id<=0)
        {
        console.error("El libro consultado no se encuentra registrado!","Error 3734334");
        navigate("/");
      }

      setBook(data);

    } catch (error) {
        setError(error);
      setError(`Error 6436: ${error}`);
      console.error('Error fetching libros:', error);
    }
  };
  
  const fetchReviewslibro = async (libro: Libro) => {
    try {
        let url = `${apiUrl}/${CtrlNameReview}/ConsultarReviewsPorLibro`;

        const response = await fetch(url, {
            method: 'POST', // Método HTTP
            headers: {
              'Content-Type': 'application/json', // Indicar que el cuerpo es JSON
            },
            body: JSON.stringify(libro), // Convertir el objeto a JSON
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
        setError(error);
        console.error('Error fetching reviews:', error);
        setReviewsLoading(false);
    }
  };


useEffect(() => {
  const loadConfig = async () => {
  await fetchConfig(setAPIUrl, setError, setLoadingConfig);
  };
  loadConfig();
}, [location.pathname]);

useEffect(() => {
    const fetchInitialData = async () => {
      try {
        
        // Obtener datos del libro
        if (!apiUrl) {
            return
        }
        
        // Obtener email del usuario desde el token
        const token = GetAccessToken();

        if (token) {
          const decoded = jwtDecode<JwtPayload>(token);
          setUserId(decoded.id);
          setUserEmail(decoded.correo);
          setUserNombre(decoded.nombre);
        }

        fetchLibroid(idlibro);         
        
        setLoading(false);
      } catch (error) {
        console.error('Error fetching data:', error);
        setError(error);
        setLoading(false);
      }
    };

    fetchInitialData();
  }, [apiUrl,idlibro]);

   // Fetch reseñas independiente
   useEffect(() => {
 
    if (!loading && book) { // Solo ejecutar cuando el libro esté cargado
        fetchReviewslibro(book);
    }
  }, [idlibrostr, loading, book]);

  const handleReviewSubmit = async (newReviewdata: { rating: number; comment: string })=> {
        
    let response;
        const token = GetAccessToken();

        if (!token) 
            {
                console.error("No puede realizar una reseña sin estar logueado!","Error 21415");
                navigate("/Login");
                return;
            }

            const decoded = jwtDecode<JwtPayload>(token);
            setUserId(decoded.id);
            setUserEmail(decoded.correo);
            setUserNombre(decoded.nombre);

            const user : Usuario = {
                id:userId,
                nombre: userNombre,
                correo : userEmail,
                password : "",             
            }

            const newReview : Review = {
                id: 0,
                calificacion: newReviewdata.rating,
                comentario: newReviewdata.comment,
                createdAt: new Date(),
                libro:book,
                usuario: user

            };

        try{
            const requestOptions: RequestOptions = {
                method: 'POST',
                body: newReview };
        response = await fetchWithAuth<void>(`${apiUrl}/${CtrlNameReview}/ConsultarReviewsPorLibro`, token,requestOptions);
        }catch (error) {
            if (error instanceof SessionExpiredError) {
                console.error(error.message); // Mensaje: "Su sesión ha vencido. Debe loguearse de nuevo!"
                navigate('/login'); // Redirigir al login
              } else {
                console.error('Error 62362 durante envío de reseña:', error);
            }
        }

      if (response.ok) {
        // Refrescar las reseñas después de enviar
        const updatedReviewsResponse = await  fetchReviewslibro(book);
       }
       else{
        console.error('Error 823682 No se realizó el envío de la reseña:', response.text);
       }
    };

  if (loadingConfig) {
    return <div>Cargando configuración...</div>;
  }
  if (loading && !book) return <div>Cargando libro...</div>;
  if (!loading && !book) return <div>Libro no encontrado</div>;
  if (error) {
    return <div>{error}</div>;
  }


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
          {userEmail ? (
            <ReviewForm 
              onSubmit={handleReviewSubmit} 
              userEmail={userEmail} 
            />
          ) : (
            <div className="bg-yellow-100 p-4 rounded-lg mb-6">
              <p>
                Debes <button 
                  onClick={() => navigate('/Login')} 
                  className="text-blue-500 hover:underline"
                >
                  iniciar sesión
                </button> para comentar
              </p>
            </div>
          )}
    
          {reviewsLoading ? (
            <div>Cargando reseñas...</div>
          ) : (
            <ReviewList reviews={reviews} />
          )}
        </div>
      );
    };