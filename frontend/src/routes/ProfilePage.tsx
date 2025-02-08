import React, { useEffect }  from 'react';
import { useNavigate } from 'react-router-dom';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { useAppContext} from '../components/AppContext.tsx';

export default function ProfilePage() {
    const navigate = useNavigate(); // Hook para navegar entre rutas
    const context:AppContextType = useAppContext(); // Extraer valores del contexto

    useEffect(() => {
        if(!context.token){
          console.log("No hay usuario logueado");
          navigate('/');
          return;
        }
        context.GuardarDatosUser(context.token);
    }, []); 

    return (
      <div className="mainContainer">
        <div className="content-wrapper">
          {/* Contenido principal */}
          <div className="content p-4">
            <p>{context.user?.correo}</p>
            <p>{context.user?.nombre}</p>
            <p>{context.user?.fotoPerfil}</p>
          </div>
        </div>
      </div>
    );
  }