// authUtils.ts
import { NavigateFunction } from "react-router-dom";
import { AppContextType } from "../Interfaces/AppContextType"; 

export const validarUsuario = (idUsuario: string | undefined, context: AppContextType, navigate: NavigateFunction) => {

    if (!context.token) {
        console.log("No hay usuario logueado");
        navigate("/");
        return false;
    }
    if (!context.user) {
        console.log("No hay datos del usuario logueado");
        navigate("/");
        return false;
    }
    if(!idUsuario){
        console.log("ID no definido");
        navigate('/');
        return false;
    }

    const id = +idUsuario;

    if (!id || id <= 0) {
    console.log("ID inválido");
    navigate("/");
    return false;
    }
    if (context.user.id !== id) {
    console.log("Usuario no coincide");
    navigate("/");
    return false;
  }

  return true;
};
