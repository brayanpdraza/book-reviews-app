import React, { useEffect }  from 'react';
import { useNavigate } from 'react-router-dom';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { useAppContext} from '../components/AppContext.tsx';

export default function ProfilePage() {
    const navigate = useNavigate(); // Hook para navegar entre rutas
    const context:AppContextType = useAppContext(); // Extraer valores del contexto
    const urlDefault = "https://www.gravatar.com/avatar/?d=mp";
    
    useEffect(() => {
      if(!context.isAuthenticated){
        console.log("No hay sesion iniciada");
        navigate('/');
        return;
      }
        if(!context.token){
          console.log("No hay usuario logueado");
          navigate('/');
          return;
        }
    }, []); 

    const getProfileImage = () => {
      if (!context.user?.fotoPerfil) return urlDefault; // Imagen por defecto
      return `data:image/jpeg;base64,${context.user?.fotoPerfil}`; // Ajusta el tipo MIME si es necesario (ej: image/png)
    };
  
    return (
      <div className="mainContainer">
        <div className="content-wrapper">
          {/* Contenido principal */}
          <div className="content p-4">
            <p>{context.user?.correo}</p>
            <p>{context.user?.nombre}</p>
            <img
              src={getProfileImage()}
              alt="Foto de perfil"
              className="w-40 h-40 object-cover cursor-pointer border-2 border-blue-300 shadow-lg"
              onError={(e) => {
                if (e.target.src !== urlDefault) {
                    e.target.src = urlDefault;
                  }
              }}
            />
          </div>
        </div>
      </div>
    );
  }