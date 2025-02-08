import { AutenticacionRes } from "../Interfaces/AuthenticationRes.ts";
import { ResponseErrorGet } from './ResponseErrorGet.ts';

// Función modificada para recibir la función "login" del contexto
export const LoginMethod = async (
  apiUrl: string,
  ControllerName: string,
  email: string,
  password: string,
  navigate: Function,
  login: (token: string, refreshToken: string) => void // <- Función del contexto
) => {
  try {
    const url = `${apiUrl}/${ControllerName}/AutenticacionUsuarioPorCorreoYPassword/${email}/${password}`;
    const response = await fetch(url);

    if (!response.ok) { 
      const errorContent = await ResponseErrorGet(response);
      throw new Error(`Error ${response.status}: ${errorContent}`);
    }
    const data: AutenticacionRes = await response.json();
    
    await login(data.credential, data.renewalCredential);
    
    console.log("Ha iniciado sesión");
    navigate('/');
  } catch (error) {
    console.error('Error en la solicitud de Login:', error);
    throw error;
  }
};