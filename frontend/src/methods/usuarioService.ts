import { Usuario } from "../Interfaces/Usuario";

export const obtenerUsuarioPorId = async (apiUrl:string, ControllerName:string,id: number,): Promise<Usuario | null> => {
  try {
    const url = `${apiUrl}/${ControllerName}/ObtenerUsuarioid/${id}`;
    const response = await fetch(url);
    if (!response.ok) throw new Error("No se pudo obtener la información del usuario");

    return await response.json();
  } catch (error) {
    console.error("Error obteniendo información del usuario:", error);
    return null;
  }
};
