
import {setAccessToken} from './SetAccessToken.ts';
import {SessionExpiredError} from './SessionExpiredError.ts';
import {RequestOptions} from '../Interfaces/RequestOptions.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';
import {getRefreshToken} from '../methods/getRefreshToken.ts';

  interface RefreshTokenResponse {
        Credential:string;
        RenewalCredential :string;
        DateTime: string;
  }


  export const fetchWithAuth = async <T>(
    url: string, 
    token: string, 
    { method = 'POST', body = null }: RequestOptions = {},
    retryCount = 0 // Contador de reintentos
  ): Promise<Response> => {
    const headers: HeadersInit = {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    };
  
    const options: RequestInit = {
      method,
      headers,
      body: body ? JSON.stringify(body) : null, // Incluimos el cuerpo si existe
      //credentials: 'include' // Necesario para cookies HttpOnly
    };
    try {
      const response = await fetch(url, options);
  
      if (response.status === 204) {
        return response; // Respuesta exitosa sin contenido
      }
  
      if (!response.ok) {
        if (response.status === 401 && retryCount < 1) {
          // Intentar renovar el token si el intento es menor a 1
          const newToken = await refreshAuthToken();
          if (!newToken) throw new SessionExpiredError(); // Si no se puede renovar, error
          return fetchWithAuth<T>(url, newToken, { method, body }, retryCount + 1);
        } else {
          const errorContent = await ResponseErrorGet(response);
          console.error('Error en la solicitud:', errorContent);
          throw new Error(`Error ${response.status}: ${errorContent}`);
        }
      }
  
      return response; // Respuesta exitosa
    } catch (error) {
      // Manejo de errores por excepción directa (fetch falló antes de recibir respuesta)
      if (retryCount < 1 && error ) {
        console.warn('Error al hacer fetch, intentando renovar token:', error);
  
        // Intentar renovar el token
        const newToken = await refreshAuthToken();
        if (!newToken) throw new SessionExpiredError(); // Si no se puede renovar, error
        return fetchWithAuth<T>(url, newToken, { method, body }, retryCount + 1);
      }
  
      // Si ya se intentó renovar o es otro error, lanzar el error original
      console.error('Request failed:', error);
      throw error;
    }
    
  };
    
    // Función para refrescar el token
    export const refreshAuthToken = async (): Promise<string | null> => {

      const refreshToken = getRefreshToken();
      console.log(refreshToken);

      if (!refreshToken) {
        console.error('No refresh token available');
        return null;
      }
    
      const urlrefreshtoken= "http://localhost:1212/Usuario/update-refresh-token";

    
      try {
        const response = await fetch(urlrefreshtoken, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ refreshToken }),
        });
    
        if (!response.ok) {
          throw new Error('Failed to refresh token');
        }
    
        const data: RefreshTokenResponse = await response.json();
        const newToken = data.Credential;
    
        // Guarda el nuevo token 
        setAccessToken(newToken);
    
        return newToken;
      } catch (error) {
        console.error('Token refresh failed', error);
        redirectToLogin(); // Redirige al usuario al login si la renovación falla
        return null;
      }
    };

    // Redirigir al login
const redirectToLogin = () => {
    // Limpiar cualquier dato relacionado con la sesión
    setAccessToken("");
    localStorage.removeItem('refreshToken');
    
    // Redirigir a la página de login
    window.location.href = '/Login'; // Aquí puedes redirigir al login o la página que desees
  };