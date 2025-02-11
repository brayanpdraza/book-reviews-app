
import {SessionExpiredError} from './SessionExpiredError.ts';
import {RequestOptions} from '../Interfaces/RequestOptions.ts';
import {AppContextType} from '../Interfaces/AppContextType.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';
import { AutenticacionRes } from '../Interfaces/AuthenticationRes.ts';
import { getRefreshToken } from './getRefreshToken.ts';
import { setAccessToken } from './SetAccessToken.ts';
import { setRefreshToken } from './SetRefreshToken.ts';

export const fetchWithAuth = async <T>(
  url: string,
  token: string | null,
  { method = 'POST', body = null }: RequestOptions = {},
  retryCount = 0,
  context: AppContextType // <--- Aceptar el contexto
): Promise<Response> => {
  const headers: HeadersInit = {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
  };

    if(context.isLoggingOut){
        throw new Error("El usuario se está deslogueando");
    }

    if(!context.isAuthenticated){
        throw new Error("El usuario no está autenticado");
    }  

    const options: RequestInit = {
      method,
      headers,
      body: body ? JSON.stringify(body) : null,
    };

    try {

      const response = await fetch(url, options);

      // Caso: Respuesta exitosa sin contenido
      if (response.status === 204) return response;

      // Manejo de errores HTTP
      if (!response.ok) {
          // Caso especial: Token expirado
          if (response.status === 401 && retryCount < 1) {

              const newToken = await refreshAuthToken(context); // <--- Pasar el contexto
              if (!newToken) throw new SessionExpiredError();
              return fetchWithAuth<T>(url, newToken, { method, body }, retryCount + 1, context);
          }

          const errorContent = await ResponseErrorGet(response);
          throw new Error(`HTTP Error ${response.status}: ${errorContent}`);
      }

      return response;

    } catch (error) {
      // Manejo de errores de red
        if (error instanceof TypeError && retryCount < 1) {
            console.warn('Network error detected, retrying...', retryCount+1); 
            const newToken = await refreshAuthToken(context); // <--- Pasar el contexto
        if (!newToken) throw new SessionExpiredError();
            console.warn('retrying Succeful');
            return fetchWithAuth<T>(url, newToken, { method, body }, retryCount + 1, context);
        }
            // Relanzar errores no manejados
            if (error instanceof SessionExpiredError) {
            context.handleError(error);
        }

      throw error;
  }
};

export const refreshAuthToken = async (context: AppContextType): Promise<string | null> => {
      // 1. Obtener refresh token del storage
      if (!context.refreshToken) return null;

      // 2. Hacer la solicitud para renovar el token
      const response = await fetch('http://localhost:1212/Usuario/update-refresh-token', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(context.refreshToken),
      });

      // 3. Manejar errores de la respuesta
      if (!response.ok) {
          const errorContent = await ResponseErrorGet(response);
          throw new Error(`Error al renovar el token: ${errorContent}`);
      }

      // 4. Extraer los nuevos tokens
      const data: AutenticacionRes = await response.json();

      // 5. Actualizar el storage
      setAccessToken(data.credential);
      setRefreshToken(data.renewalCredential);

      // 6. Sincronizar con el contexto
      context.setToken(data.credential);
      context.setRefreshToken(data.renewalCredential);
      console.log("credentials renewed")
      // 7. Retornar el nuevo access token
      return data.credential;

};
