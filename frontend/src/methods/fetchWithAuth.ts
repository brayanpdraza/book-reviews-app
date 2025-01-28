
import {setAccessToken} from './SetAccessToken.ts';
import {SessionExpiredError} from './SessionExpiredError.ts';
import {RequestOptions} from '../Interfaces/RequestOptions.ts';
import {ResponseErrorGet} from '../methods/ResponseErrorGet.ts';

  interface RefreshTokenResponse {
        Credential:string;
        RenewalCredential :string;
        DateTime: string;
  }


  export const fetchWithAuth = async <T>(
    url: string, 
    token: string, 
    { method = 'POST', body = null }: RequestOptions = {}
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
    let response;
    let newToken
    try {
      response = await fetch(url, options);
    } catch (error) {
      console.error('Request failed', error); //FALLO
      throw error;
    }
      if (response.status === 204) {
        // Si la respuesta es 204 No Content, no hay cuerpo, pero se considera exitoso
        return response; // Retornamos el response directamente
      }
  
      if (!response.ok) {
        // Si obtenemos un 401 Unauthorized, intentamos refrescar el token
        if (response.status === 401) {
          try{
               const errorContent = await ResponseErrorGet(response);
                    console.log(errorContent);  
          newToken = await refreshAuthToken();
          }catch(error){
            throw new SessionExpiredError();
          }
          if (!newToken) {
            throw new SessionExpiredError();
            
          }
        } 
        return fetchWithAuth<T>(url, newToken, { method, body });
      }
  
      return response; // Retornamos la respuesta completa (para manejarla más adelante)
    
  };
    
    // Función para refrescar el token
    const refreshAuthToken = async (): Promise<string | null> => {

      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) {
        console.error('No refresh token available');
        return null;
      }
    
      const urlrefreshtoken= "http://localhost:1212/usuario/update-refresh-token";


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
    
        // Guarda el nuevo token en localStorage o donde sea necesario
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