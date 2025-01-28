export const ResponseErrorGet = async (response : Response) => {

        let Error;
        const errorContent = await response.text();       
        try {
          const errorJson = JSON.parse(errorContent);
          Error = errorJson.message || 'Error desconocido';
        } catch {
          // Si falla el parseo, mostrar el texto plano
          Error = errorContent || `Error ${response.status}: ${response.statusText}`;
        }
        return Error;
}