export const ResponseErrorGet = async (response: Response) => {
  let Error;
  const errorContent = await response.text(); 
  
  console.log("Contenido de error recibido:", errorContent);

  if (!errorContent.trim() || errorContent.startsWith("<!DOCTYPE html>")) {  
      return `Error ${response.status}: ${response.statusText}`;
  }
  try {
      const errorJson = JSON.parse(errorContent);
      Error = errorJson.message || "Error desconocido";
  } catch {
      Error = errorContent || `Error ${response.status}: ${response.statusText}`;
  }
  return Error;
};