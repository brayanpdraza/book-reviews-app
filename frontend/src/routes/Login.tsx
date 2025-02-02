import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {LoginMethod} from "../methods/LoginMethod.ts";
import { useAppContext} from '../components/AppContext.tsx';
import { AppContextType} from '../Interfaces/AppContextType.ts';

export default function Login() {
  const context: AppContextType = useAppContext();

    const [emailForm, setEmailForm] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();
    const ControllerName = "Usuario";


    
    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    if (emailForm.length < 8) {
        setError('Debe ingresar un correo electrónico válido');
        setLoading(false);
        return;
      }
      if (!emailForm.includes('@')){
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
      if (!context.apiUrl)
        throw new Error("Error de configuración. Cargue la página de nuevo");

      await LoginMethod(context.apiUrl,ControllerName,emailForm,password,navigate,context.login);
      console.log("Ha iniciado sesión");
    } catch (error) {
      setError((error && error.message) || "error Inicio Sesion");
        console.error('Error 563873:', error);
    } finally {
      setLoading(false);
    }
  };

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
            value={emailForm}
            onChange={(e) => setEmailForm(e.target.value)}
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