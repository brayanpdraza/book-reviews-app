import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppContext } from './AppContext.tsx'
import { AppContextType} from '../Interfaces/AppContextType.ts';

const HeaderLoginUser = () => {
  const context:AppContextType = useAppContext();
  const navigate = useNavigate(); // Usamos useNavigate para redirigir



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
              <>
                <button className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors">
                  {context.user.correo}
                </button>
                <button
                  onClick={context.handleLogout}
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