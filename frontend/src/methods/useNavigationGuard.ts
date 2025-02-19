// useNavigationGuard.ts
import { useEffect, useCallback } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

export const useNavigationGuard = (hasUnsavedChanges: boolean) => {
    const navigate = useNavigate();
    const location = useLocation();
    const currentPath = location.pathname;

  const confirmExit = useCallback(() => {
    if (!hasUnsavedChanges) return true;
    return window.confirm('Tienes cambios sin guardar. ¿Seguro que quieres salir?');
    }, [hasUnsavedChanges]);


    useEffect(() => {
                    console.log("IM Here:"+ hasUnsavedChanges);
        const handleBeforeUnload = (e) => {
            if (hasUnsavedChanges) {
                e.preventDefault();
                e.returnValue = "";
            }
        };

        const handleRouteChange = (e) => {
            if (hasUnsavedChanges) {
                const confirmExit = window.confirm("Tienes cambios sin guardar. ¿Seguro que quieres salir?");
                if (!confirmExit) {
                    e.preventDefault(); // Evita la navegación
                } else {
                    navigate(e.location.pathname); // Realiza la navegación
                }
            }
        };

        window.addEventListener("beforeunload", handleBeforeUnload);
        window.addEventListener("popstate", handleRouteChange);

        return () => {
            window.removeEventListener("beforeunload", handleBeforeUnload);
            window.removeEventListener("popstate", handleRouteChange);
        };
    }, [hasUnsavedChanges, navigate]);

  return { confirmExit };
};