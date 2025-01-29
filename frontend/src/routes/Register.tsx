import React, { useState,useEffect } from 'react';
import { useNavigate,useLocation } from 'react-router-dom';
import { setAccessToken } from '../methods/SetAccessToken.ts';
import { GetAccessToken } from '../methods/GetAccessToken.ts';
import { fetchConfig } from '../methods/fetchConfig.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';
import {Usuario} from '../Interfaces/Usuario.ts';
import {getRefreshToken} from '../methods/getRefreshToken.ts';
import {setRefreshToken} from '../methods/SetRefreshToken.ts';

interface AutenticacionRes{
    credential:string;
    renewalCredential:string;
    expiry:Date;
}


export default function Register(){

    const [nombre, setNombre] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [loadingConfig, setLoadingConfig] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
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
    
        // Validaciones
        if (nombre.length <= 0) {
          setError('Debe ingresar el nombre');
          setLoading(false);
          return;
        }
    
        if (email.length < 8 || !email.includes('@')) {
          setError('Debe ingresar un correo electrónico válido');
          setLoading(false);
          return;
        }
    
        if (password.length < 8) {
          setError('La contraseña debe tener al menos 8 caracteres');
          setLoading(false);
          return;
        }
    
        if (password !== confirmPassword) {
          setError('Las contraseñas no coinciden');
          setLoading(false);
          return;
        }
        
            //REGISTRO USUARIO

            try {
            const url = `${apiUrl}/${ControllerName}/GuardarUsuario`;

            const usuario: Usuario = { 
                id : 0,
                nombre: nombre, 
                correo : email, 
                password: password,
            };
        
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                'Content-Type': 'application/json',
                },
                body: JSON.stringify(usuario),
            });
        
            if (!response.ok) {
                const errorContent = await ResponseErrorGet(response);
                setError(errorContent);
                return;
            }
        
            
            } catch (error) {
                setError(error instanceof Error ? error.message : "Error en el registro");
                console.error('Error en registro:', error);
            }

            //INICIO SESION
            try {
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
          <h1 className="text-2xl font-bold mb-6 text-gray-800">Registro de Usuario</h1>
    
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">
                Nombre Completo
              </label>
              <input
                type="text"
                id="nombre"
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                disabled={loading}
              />
            </div>
    
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
    
            <div className="relative">
              <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                Confirmar Contraseña
              </label>
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                id="confirmPassword"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                disabled={loading}
              />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute right-2 top-8 p-1 text-gray-500 hover:text-blue-500"
              >
                {showConfirmPassword ? '👁️' : '👁️‍🗨️'}
              </button>
            </div>
    
            {error && (
              <div className="text-red-600 text-sm">
                <p>{error}</p>
              </div>
            )}
    
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 disabled:bg-blue-300 disabled:cursor-not-allowed"
            >
              {loading ? 'Registrando...' : 'Registrarse'}
            </button>
          </form>
    
          <div className="mt-4 text-center">
            <p className="text-sm text-gray-600">
              ¿Ya tienes cuenta?{' '}
              <button
                onClick={() => navigate('/login')}
                className="text-blue-600 hover:underline"
              >
                Inicia sesión aquí
              </button>
            </p>
          </div>
        </div>
      );
}