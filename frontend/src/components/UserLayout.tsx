import React, { useEffect } from 'react';
import { Outlet, useParams, useNavigate } from 'react-router-dom';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { useAppContext} from '../components/AppContext.tsx';
import { validarUsuario } from '../methods/AuthUtils.ts';

const UserLayout = () => {
  const { idUsuario } = useParams();
  const navigate = useNavigate();  
  const context:AppContextType = useAppContext();

  useEffect(() => {
    validarUsuario(idUsuario, context, navigate);
  }, [idUsuario, navigate,context.user]);

  return (
    <div className="min-h-screen bg-gray-100">

      <header className="bg-blue-500 text-white p-4">
        <h1 className="text-2xl font-bold">Área de Usuario</h1>
      </header>

      <main className="p-6">
        <Outlet />
      </main>

    </div>
  );
};

export default UserLayout;