import React, { useState,useEffect } from 'react';
import { useNavigate,useLocation } from 'react-router-dom';
import { setAccessToken } from '../methods/SetAccessToken.ts';
import { GetAccessToken } from '../methods/GetAccessToken.ts';
import { fetchConfig } from '../methods/fetchConfig.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';
import {getRefreshToken} from '../methods/getRefreshToken.ts';
import {setRefreshToken} from '../methods/SetRefreshToken.ts';

interface AutenticacionRes{
    credential:string;
    renewalCredential:string;
    expiry:Date;
}

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [loadingConfig, setLoadingConfig] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState(false);
     const [apiUrl, setAPIUrl] = useState('');
    const navigate = useNavigate();
    const location = useLocation();
    const ControllerName = "Usuario";

   useEffect(() => {
      const token = GetAccessToken();
      const refreshtoken = getRefreshToken();
          if (token && refreshtoken) {
            console.log("Ya cuenta con una sesion iniciada");
            navigate('/');
            return;
        }

        const loadConfig = async () => {
          await fetchConfig(setAPIUrl, setError, setLoadingConfig);
        };
        loadConfig();
      
      }, [location.pathname]);
    

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");


    if (email.length < 8) {
        setError('Debe ingresar un correo electrónico válido');
        setLoading(false);
        return;
      }
      if (!email.includes('@')){
        setError('Correo electrónico inválido');
        setLoading(false);
        return;
      }
  
      if (password.length < 8) {
        setError('La contraseña debe tener al menos 8 caracteres');
        setLoading(false);
        return;
      }


    try {
        //REFACTORIZAR
        let url = `${apiUrl}/${ControllerName}/AutenticacionUsuarioPorCorreoYPassword/${email}/${password}`;

        const response = await fetch(url);

        if (!response.ok) { 
            const errorContent = await ResponseErrorGet(response);
            setError(errorContent);   
            
            return;
          }
          

          const data: AutenticacionRes = await response.json();
          setAccessToken(data.credential);
          setRefreshToken(data.renewalCredential);
          navigate('/');

    } catch (error) {
        setError([error || "error Inicio Sesion"]);
        console.error('Error 563873:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loadingConfig) {
    return <div>Cargando configuración...</div>;
  }

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white rounded-lg shadow-md">
      <h1 className="text-2xl font-bold mb-6 text-gray-800">Iniciar Sesión</h1>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700">
            Correo electrónico
          </label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            disabled={loading}
          />
        </div>

        <div className="relative">
          <label htmlFor="password" className="block text-sm font-medium text-gray-700">
            Contraseña
          </label>
          <input
            type={showPassword ? 'text' : 'password'}
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            disabled={loading}
          />
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="absolute right-2 top-8 p-1 text-gray-500 hover:text-blue-500"
          >
            {showPassword ? '👁️' : '👁️‍🗨️'}
          </button>
        </div>

        {error && ( // Mostrar un solo mensaje de error
          <div className="text-red-600 text-sm">
            <p>{error}</p>
          </div>
        )}

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 disabled:bg-blue-300 disabled:cursor-not-allowed"
        >
          {loading ? 'Cargando...' : 'Iniciar Sesión'}
        </button>
      </form>

      <div className="mt-4 text-center">
        <p className="text-sm text-gray-600">
          ¿No tienes cuenta?{' '}
          <button
            onClick={() => navigate('/registro')}
            className="text-blue-600 hover:underline"
          >
            Regístrate aquí
          </button>
        </p>
      </div>
    </div>
  );
}