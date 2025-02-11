import React, { useState, useRef, useEffect,useCallback } from "react";
import { useAppContext  } from './AppContext.tsx'
import { useNavigate,useLocation } from "react-router-dom"; // Asegúrate de tener react-router-dom instalado
import { AppContextType} from '../Interfaces/AppContextType.ts';

const UserMenu = () => {

  const context:AppContextType = useAppContext();
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef(null);
  const navigate = useNavigate();
  const location = useLocation(); // Obtiene la ruta actual
  const urlDefault = "https://www.gravatar.com/avatar/?d=mp";
  
  // Cerrar menú al hacer clic fuera
  const handleClickOutside = useCallback((event) => {
    if (menuRef.current && !menuRef.current.contains(event.target)) {
      setIsOpen(false);
    }
  }, []);
  
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [handleClickOutside]);

  // Generar URL de la imagen desde el string binario. REFACTORIZAR A UN MÉTODO UTILITARIO CON EL BASE64 DE LA IMAGEN Y EL URL DEFAULT
  const getProfileImage = () => {
    if (!context.user?.fotoPerfil) return urlDefault; // Imagen por defecto
    return `data:image/jpeg;base64,${context.user?.fotoPerfil}`; // Ajusta el tipo MIME si es necesario (ej: image/png)
  };

  const isActive = (path: string) => location.pathname === path;


  // Refactor: Función que retorna las clases activas/inactivas
  const getMenuItemClasses = (path: string) => ({
    base: "block w-full px-4 py-2 text-sm text-left",
    active:
      isActive(path)
        ? "text-blue-600 font-semibold border-l-4 border-blue-600 bg-gray-100"
        : "text-gray-700 hover:bg-gray-100",
  });

  return (
    <div className="relative" ref={menuRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center focus:outline-none"
      >
        <img
          src={getProfileImage()}
          alt="Foto de perfil"
          className="w-20 h-20 rounded-full object-cover cursor-pointer border-2 border-gray-300"
          onError={(e) => {
            if (e.target.src !== urlDefault) {
              e.target.src = urlDefault;
            }
          }}
        />
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 ring-1 ring-black ring-opacity-5">
          {[
            { path: `/Usuario/${context.user?.id}/Perfil`, label: "Ver Perfil" },
            { path: `/Usuario/${context.user?.id}/Reviews`, label: "Mis Reseñas" },
          ].map(({ path, label }) => (
            <button
              key={path}
              onClick={() => {
                navigate(path);
                setIsOpen(false);
              }}
              className={`${getMenuItemClasses(path).base} ${getMenuItemClasses(path).active}`}
            >
              {label}
            </button>
          ))}

          <button
            onClick={() => {
              context.handleLogout();
              setIsOpen(false);
            }}
            className="block w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 text-left"
          >
            Cerrar Sesión
          </button>
        </div>
      )}
    </div>
  );
};

export default UserMenu;