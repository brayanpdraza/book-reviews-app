import {ResponseErrorGet} from './ResponseErrorGet.ts';
import {Usuario} from "../Interfaces/Usuario.ts";

export const RegisterMethod = async (apiUrl:string, ControllerName: string, nombre: string, email: string, password : string) =>{

     const url = `${apiUrl}/${ControllerName}/GuardarUsuario`;

        const usuario: Usuario = { 
            id : 0,
            nombre: nombre, 
            correo : email, 
            password: password,
        };

        const response = await fetch(url, {
            method: 'POST',
            headers: {
            'Content-Type': 'application/json',
            },
            body: JSON.stringify(usuario),
        });


        if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        console.error('Error en la solicitud de Registro:', errorContent);

        if(response.status===409){
            throw new Error("El correo ingresado ya está registrado.");
        }


        throw new Error(`Error ${response.status}: ${errorContent}`);
        }
    console.log("Usuario Registrado"); //DEBE SER MENSAJE 
}