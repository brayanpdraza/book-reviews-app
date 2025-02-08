import {Usuario} from './Usuario.ts';

export interface AppContextType {
    user: Usuario | null; //
    setUser: (user: Usuario | null) => void;
    token: string | null;
    setToken: (token: string | null) => void;
    refreshToken: string | null;
    setRefreshToken: (refreshToken: string | null) => void;
    apiUrl: string;
    handleLogout: () => Promise<void>;
    login: (token: string, refreshToken: string) => void;
    loadingConfig: boolean;
    handleError: (error: Error) => void;
    GuardarDatosUser: (token: string) => void;
  }
  
