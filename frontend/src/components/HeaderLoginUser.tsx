import React, { useState, useEffect } from 'react';
import {fetchWithAuth} from '../methods/fetchWithAuth.ts';
import {jwtDecode,JwtPayload } from 'jwt-decode';
import { useNavigate,useLocation } from 'react-router-dom';
import { setAccessToken } from '../methods/SetAccessToken.ts';
import { fetchConfig } from '../methods/fetchConfig.ts';
import { GetAccessToken } from '../methods/GetAccessToken.ts';
import {SessionExpiredError} from '../methods/SessionExpiredError.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';
import { setRefreshToken } from '../methods/SetRefreshToken.ts';
import { getRefreshToken } from '../methods/getRefreshToken.ts';
import { refreshAuthToken } from '../methods/fetchWithAuth.ts';

interface MyJwtPayload extends JwtPayload {
  correo: string; // Agregar el campo "correo" que tienes en el JWT
}

const HeaderLoginUser = () => {
    const [email, setEmail] = useState(null);
    const [token, setToken] = useState(null);
    const [apiUrl, setAPIUrl] = useState('');
    const [error, setError] = useState(null);
    const [loadingConfig, setLoadingConfig] = useState(true);
    const ControllerName = "Usuario";
    const navigate = useNavigate(); // Usamos useNavigate para redirigir
    const location = useLocation();


    useEffect(() => {
      const tokenget = GetAccessToken();
      setToken(tokenget);
      const loadConfig = async () => {
        await fetchConfig(setAPIUrl, setError, setLoadingConfig);
      };
      loadConfig();
    
    }, [location.pathname]);
  
    useEffect(() => {
      const handleRequest = async () => {
        const refreshToken = getRefreshToken();
        if (!token) {
          if(!refreshToken){
            setEmail(null);
            return;
          }
          // Intentar renovar el token
          const newToken = await refreshAuthToken();
          if (!newToken) {
            setEmail(null);
            throw new SessionExpiredError()// Si no se puede renovar, error
          }; 
          }
          
          try {
            const decoded = jwtDecode<MyJwtPayload>(token); // Usa la interfaz personalizada aquí
            setEmail(decoded.correo); // Asegúrate de que el campo se llama "correo" en el JWT
          } catch (error) {
            console.error('Error decoding JWT:', error);
          }
      };
      
      if(loadingConfig){
        return;
      }
      try{
      handleRequest();
      }catch(error){
        if (error instanceof SessionExpiredError) {
          console.error(error.message); // Mensaje: "Su sesión ha vencido. Debe loguearse de nuevo!"
        } else {
          console.error('Error 3463463 :', error);
      }
      }
    }, [token]);
  
    const handleLogout = async () => {
         // Solo ejecutar cuando APIUrl esté disponible
        
      const tokenget = GetAccessToken();
      setToken(tokenget);
      if (!token) 
        {
          setEmail(null)
          setRefreshToken("");
          navigate('/login'); // Redirigir al login
        return;
      }
        try {
          const response = await fetchWithAuth<void>(`${apiUrl}/${ControllerName}/logout`, token); 
          if (!response.ok) { 
           const errorContent = await ResponseErrorGet(response);
          setError(errorContent);   
            return;
          }
          setAccessToken('');
          setRefreshToken('');
          console.log('Logout successful');
          navigate('/login'); // Redirigir al login
        } catch (error) {
          if (error instanceof SessionExpiredError) {
            console.error(error.message); // Mensaje: "Su sesión ha vencido. Debe loguearse de nuevo!"
            setRefreshToken("");
            setAccessToken("");
            navigate('/login'); // Redirigir al login
          } else {
            console.error('Error durante el logout:', error);
          }
        }
      
      };
  
    if (loadingConfig) {
      return <div>Cargando configuración...</div>;
    }
  
    if (error) {
      return <div>{error}</div>;
    }
    return (
      <div className="w-full p-6 bg-gray-50 border-b border-gray-200 shadow-sm">
        <div className="max-w-8xl mx-auto flex justify-between items-center">
          {/* Título "Home" alineado a la izquierda */}
          <h1
            className="text-3xl font-bold text-gray-800 hover:text-gray-600 transition-colors cursor-pointer"
            onClick={() => navigate('/')} // Redirige a la ruta principal
          >
            BOOK REVIEW APP BRAYAN PEDRAZA
          </h1>
    
          {/* Contenedor para los botones, alineado a la derecha */}
          <div className="flex gap-4">
            {email ? (
              <>
                <button className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors">
                  {email}
                </button>
                <button
                  onClick={handleLogout}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Cerrar sesión
                </button>
              </>
            ) : (
              <>
                <button
                  onClick={() => navigate('/Login')}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Iniciar sesión
                </button>
                <button
                  onClick={() => navigate('/Register')}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                >
                  Crear usuario
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    );
  };
  export default HeaderLoginUser;