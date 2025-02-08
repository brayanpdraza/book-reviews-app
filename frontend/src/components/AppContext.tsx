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
//import { setUserEmail } from '../methods/SetUserDataMemory.ts';
import { AppContextType } from '../Interfaces/AppContextType.ts';
import { setAccessToken } from '../methods/SetAccessToken.ts';
import { setRefreshToken } from '../methods/SetRefreshToken.ts';
//import { RemoveUserEmail } from '../methods/RemoveDataUserMemory.ts';
import { RemoveRefreshToken } from '../methods/RemoveRefreshToken.ts';
import { clearUserData,getUserFotoPerfil,getUserName,setUserFotoPerfil,setUserName } from '../methods/StorageUserService.ts';
import { obtenerUsuarioPorId } from '../methods/usuarioService.ts';
import { Usuario } from '../Interfaces/Usuario.ts';

interface MyJwtPayload extends JwtPayload {
  id : number;
  correo: string;
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
  }, []);
  
  // Efecto para manejar redirecciones
  useEffect(() => {
    // Si ya se está cargando la configuración, no hacemos nada.
    if (loadingConfig) return;
  
    const initAuth = async () => {
  
      // Obtenemos los tokens de forma local
      var accessToken = GetAccessToken();
      const refreshTokenLocal = getRefreshToken();
      var FotoPerfil = getUserFotoPerfil();
      var NombrePerfil = getUserName();
      // Si no se obtuvo un access token, intentamos renovarlo si existe el refresh token
      if (!accessToken) {
        if (!refreshTokenLocal) {
          // Si no hay refresh token, se asume que la sesión expiró o nunca inició
          removeSession();
          return;
        }
  
        // Intentamos renovar el token
        try {
          accessToken = await refreshAuthToken(AppContext);
          if (!accessToken) {
            throw new SessionExpiredError("");
          }

        } catch (error) {
          if (error instanceof SessionExpiredError) {
            handleError(error);
          }
          console.error('Error refreshing token:', error);
          return;
        }

      }
  
      // Actualizamos los estados correspondientes
      setToken(accessToken);
      setRefreshTokenS(refreshTokenLocal);

      if(!FotoPerfil){
        FotoPerfil="";
      }
      if(!NombrePerfil){
        NombrePerfil="";
      }

      try {
        LlenarDatosUser(accessToken,FotoPerfil,NombrePerfil);
      } catch (error) {
        console.error('Error decoding JWT:', error);
      }
  
      // Verificamos si la ruta actual es de autenticación
      const isAuthPage = ['/Login', '/Register'].includes(location.pathname);
      // Si ya hay sesión iniciada y estamos en una página de auth, redirigimos al home
      if (isAuthPage && accessToken && refreshTokenLocal) {
        console.log("Ya se encuentra una sesión iniciada");
        navigate('/');
        return;
      }

    };
  
    initAuth();
  }, [location.pathname, navigate, AppContext, loadingConfig]);


  const LlenarDatosUser = (token: string, fotoPerfilData: string, nombrePerfilData: string) => {
    if (!token) {
      console.error("El token es inválido o está vacío.");
      throw new Error("El token es inválido o está vacío.");   
    }

    const decoded = jwtDecode<MyJwtPayload>(token);

    if (!decoded.id || !decoded.correo) {
      console.error("El token no contiene los datos esperados.");
      throw new Error("El token no contiene los datos esperados.");   
    }

    const user : Usuario = {
      id:+decoded.id,
      nombre: nombrePerfilData,
      fotoPerfil: fotoPerfilData,
      correo : decoded.correo,
      password : "",             
    }
    setUser(user);
    setUserFotoPerfil(fotoPerfilData);
    setUserName(nombrePerfilData);
  }


  const login = async (token: string, refreshToken: string) => {
    if (!token) {
      console.error("El token es inválido o está vacío.");
      throw new Error("El token es inválido o está vacío.");   
    }

    try{
      await GuardarDatosUser(token);
    }catch(error){
      console.error("Error al guardar los datos del usuario:", error);
      window.alert("Error al guardar los datos del usuario:"+error);
    }
    setAccessToken(token); 
    setToken(token);
    setRefreshToken(refreshToken);
    setRefreshTokenS(refreshToken);

  };

  const ObtenerDatosUser = async (token: string) => {
    // Decodificar el token para obtener el ID del usuario
    const decoded = jwtDecode<MyJwtPayload>(token);
    const id = +decoded.id;

    if (!id || id <= 0){ 
      console.error("El token no contiene un ID válido.");
      throw new Error("El ID del usuario no es válido");
    }

    return await obtenerUsuarioPorId(apiUrl,ControllerName,id);
  }

  const GuardarDatosUser = async (token: string) => {
    let user: Usuario | null = null;

    try{
      user = await ObtenerDatosUser(token);

    }catch(error){
      throw new error("Error al obtener los datos del usuario:"+error);
    }

    if(!user){
      throw new error("Error luego de consultar datos: El usuario es nulo");
    }
    LlenarDatosUser(token, user?.fotoPerfil ?? "", user?.nombre ?? "");
  }

  const removeSession = () => {
    setAccessToken("");
    setToken("");
    RemoveRefreshToken();
    setRefreshTokenS(null);
    clearUserData();
    setUser(null);
  };

  // Dentro del provider:
  const handleError = (error: Error) => {
    if (error instanceof SessionExpiredError) {
      window.alert("Sesion Expirada. Debe iniciar sesión de nuevo.");
      removeSession(); // <- Usa la función de logout del contexto
      navigate('/Login'); // <- Navegación manejada por el 
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
      handleError,
      GuardarDatosUser,
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