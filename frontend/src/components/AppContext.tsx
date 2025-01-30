import React, { createContext, useState, useEffect, useContext,ReactNode } from 'react';
import { useNavigate, useLocation ,} from 'react-router-dom';
import { fetchWithAuth } from '../methods/fetchWithAuth.ts';
import {jwtDecode, JwtPayload } from 'jwt-decode';
import { fetchConfig } from '../methods/fetchConfig.ts';
import { GetAccessToken } from '../methods/GetAccessToken.ts';
import { SessionExpiredError } from '../methods/SessionExpiredError.ts';
import { ResponseErrorGet } from '../methods/ResponseErrorGet.ts';
import { getRefreshToken } from '../methods/getRefreshToken.ts';
import { refreshAuthToken } from '../methods/fetchWithAuth.ts';
import { setUserEmail } from '../methods/SetUserDataMemory.ts';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { setAccessToken } from '../methods/SetAccessToken.ts';
import { setRefreshToken } from '../methods/SetRefreshToken.ts';
import { RemoveUserEmail } from '../methods/RemoveDataUserMemory.ts';
import { RemoveRefreshToken } from '../methods/RemoveRefreshToken.ts';
import { Usuario } from '../Interfaces/Usuario.ts';

interface MyJwtPayload extends JwtPayload {
  id : number;
  correo: string;
  nombre: string;
}

interface AppProviderProps {
  children: ReactNode;
}

const AppContext = createContext<AppContextType | null>(null);

export const AppProvider: React.FC<AppProviderProps> = ({ children }) => {
  const [user, setUser] = useState<Usuario | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [refreshTokenS, setRefreshTokenS] = useState<string | null>(null);
  const [apiUrl, setAPIUrl] = useState('');
  const [error, setError] = useState(null);
  const [loadingConfig, setLoadingConfig] = useState(true);
  const ControllerName = "Usuario";
  const navigate = useNavigate(); // Usamos useNavigate para redirigir
  const location = useLocation();

  useEffect(() => {
    const loadConfig = async () => {
      await fetchConfig(setAPIUrl, setError, setLoadingConfig);
    };
    loadConfig();
  }, [location.pathname]);
  
  // Efecto para manejar redirecciones
  useEffect(() => {
    if (loadingConfig) return;
    setLoadingConfig(true);
    const tokenget = GetAccessToken();
    setToken(tokenget);
    const refreshTokenMem = getRefreshToken();
    setRefreshTokenS(refreshTokenMem)

    const isAuthPage = ['/Login', '/Register'].includes(location.pathname);
    if(isAuthPage && token && refreshTokenS){
      console.log("Ya se encuentra una sesion inciada");
      navigate('/');
    }

  }, [loadingConfig, navigate, location.pathname]);

  useEffect(() => {
    const handleRequest = async () => {
      const refreshTokenMem = getRefreshToken();
      setRefreshTokenS(refreshTokenMem);
      if (!token) {
        if (!refreshTokenMem) {
          RemoveUserEmail();
          setUser(null);
          return;
        }
        // Intentar renovar el token
        const newToken = await refreshAuthToken(AppContext);
        if (!newToken) {
          throw new SessionExpiredError("") 
        };
      }

      try {
        const decoded = jwtDecode<MyJwtPayload>(token);
        setUserEmail(decoded.correo)
        const user : Usuario = {
          id:decoded.id,
          nombre: decoded.nombre,
          correo : decoded.correo,
          password : "",             
        }
        setUser(user);
        console.log(user);
      } catch (error) {
        console.error('Error decoding JWT:', error);
      }
    };

    if (loadingConfig) {
      return;
    }
    try {
      handleRequest();
    } catch (error) {
      if (error instanceof SessionExpiredError) {
        handleError(error);
      } else {
        console.error('Error 3463463 :', error);
      }
    }
  }, [token]);

  const login = async (token: string, refreshToken: string, email: string) => {
    setAccessToken(token); 
    setToken(token);
    setRefreshToken(refreshToken);
    setRefreshTokenS(refreshToken);
    const decoded = jwtDecode<MyJwtPayload>(token);
    setUserEmail(decoded.correo);
    setUserEmail(decoded.correo)
    const user : Usuario = {
      id:decoded.id,
      nombre: decoded.nombre,
      correo : decoded.correo,
      password : "",             
    }
    setUser(user);
  };

  const removeSession = () => {
    setAccessToken("");
    setToken("");
    RemoveRefreshToken();
    setRefreshTokenS(null);
    RemoveUserEmail();
    setUser(null)
  };

  // Dentro del provider:
  const handleError = (error: Error) => {
    if (error instanceof SessionExpiredError) {
      setError("Sesion Expirada. Debe iniciar sesión de nuevo");
      removeSession(); // <- Usa la función de logout del contexto
      navigate('/login'); // <- Navegación manejada por el 
    }
  };


  const handleLogout = async () => {
    const tokenget = GetAccessToken();
    setToken(tokenget);

    if (!token) {
      removeSession();
      return;
    }
    try {
      const response = await fetchWithAuth<void>(`${apiUrl}/${ControllerName}/logout`, token, {},undefined,AppContext);
      if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        setError(errorContent);
        return;
      }
      console.log('Logout successful');
      removeSession();
    } catch (error) {
      if (error instanceof SessionExpiredError) {
        handleError(error);
      } else {
        console.error('Error durante el logout:', error);
        removeSession();
      }
    }

    window.alert("Ha cerrado la sesión");
    
  };


  return (
    <AppContext.Provider value={{
      user, 
      setUser, 
      token, 
      setToken, 
      refreshToken: refreshTokenS, 
      setRefreshToken: setRefreshTokenS, 
      apiUrl, 
      handleLogout, 
      login, 
      removeSession, 
      loadingConfig, 
      handleError
    }}>
      {children}
    </AppContext.Provider>
  );
};
export const useAppContext = () => {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error("useAppContext must be used within an AppProvider");
  }
  return context;
};