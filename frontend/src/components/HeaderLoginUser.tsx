import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAppContext } from './AppContext.tsx';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import UserMenu from './UserMenu.tsx';

const HeaderLoginUser = () => {
  const context: AppContextType = useAppContext();
  const navigate = useNavigate();
  const location = useLocation();

  // Función para determinar si la ruta pasada es la actual
  const isActive = (path: string) => location.pathname === path;

  // Función para obtener las clases CSS para el botón del header.
  // Se aplican estilos distintos a los del submenu, pero usando la misma lógica.
  const getHeaderButtonClasses = (path: string) => {
    const base = "px-4 py-2 rounded-lg transition-colors";
    const active = isActive(path)
      ? "bg-blue-500 text-white"        // Estilo cuando está activo
      : "bg-gray-200 text-gray-700 hover:bg-gray-300"; // Estilo inactivo
    return `${base} ${active}`;
  };

  return (
    <div className="w-full p-6 bg-gray-50 border-b border-gray-200 shadow-sm">
      <div className="max-w-8xl mx-auto flex justify-between items-center">
        <h1
          className="text-3xl font-bold text-gray-800 hover:text-gray-600 transition-colors cursor-pointer"
          onClick={() => navigate('/')}
        >
          BOOK REVIEW APP BRAYAN PEDRAZA
        </h1>
    
        <div className="flex gap-4">
          {context.user ? (
            // Si el usuario está logueado, se muestra el UserMenu
            <UserMenu />
          ) : (
            // Si no está logueado, se muestran los botones de Iniciar sesión y Crear usuario
            <>
              <button
                onClick={() => navigate('/Login')}
                className={getHeaderButtonClasses('/Login')}
              >
                Iniciar sesión
              </button>
              <button
                onClick={() => navigate('/Register')}
                className={getHeaderButtonClasses('/Register')}
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