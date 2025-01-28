import { useNavigate,useRouteError } from 'react-router-dom';
import React from 'react';

export default function Error(){
    const navigate = useNavigate();
    const error = useRouteError();

    const handleRedirect = () => {
        navigate('/'); // Aquí defines la ruta a la que quieres redirigir
    };

    const errorMessage = (() => {
        if (error instanceof Response) {
            return `Error ${error.status}: ${error.statusText}`;
        } else if (error && typeof error === "object" && "message" in error) {
            return (error as Error).message; // Mensaje para errores de tipo Error
        } else {
            return "Error desconocido";
        }
    })();

    return(
        <div>
            <h1>Error, ruta no existente</h1>
            <p>{errorMessage}</p>
            <button onClick={handleRedirect}>Regresar A Inicio</button>
        </div>
    );
}