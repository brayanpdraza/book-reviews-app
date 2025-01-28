import React from 'react';

export const fetchConfig = async (setAPIUrl: React.Dispatch<React.SetStateAction<string>>, setError: React.Dispatch<React.SetStateAction<string>>, setLoadingConfig: React.Dispatch<React.SetStateAction<boolean>>) => {
    try {
      const response = await fetch('/config.json');
      if (!response.ok) {
        throw new Error('Error en la respuesta de la red');
      }
  
      const config = await response.json();
      if (!config.apiUrl) {
        throw new Error('La variable global URL está vacía o no existe');
      }
      setAPIUrl(config.apiUrl);
    } catch (error) {
      setError(`Error 125125: ${error}`);
    } finally {
      setLoadingConfig(false);
    }
  };