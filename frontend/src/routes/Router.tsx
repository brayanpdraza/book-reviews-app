import React from "react";
import { createBrowserRouter,Outlet } from "react-router-dom";
import Home from './Home.tsx';
import Error from './Error.tsx';
import Login from './Login.tsx';
import Register from './Register.tsx';
import DetalleLibro from './DetalleLibro.tsx';
import Layout from '../components/Layout.tsx'

const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />, // Usa el Layout aquí
    errorElement: <Error />,
    children: [
      {
        path: '/',
        element: <Home />, // Home ahora es una ruta hija
      },
      {
        path: '/Login',
        element: <Login />, // Login ahora es una ruta hija
      },
      {
        path: '/Register',
        element: <Register />, // Register ahora es una ruta hija
      },
      {
        path: '/DetalleLibro/:idlibro',
        element: <DetalleLibro />, // DetalleLibro ahora es una ruta hija
      },
    ],
  },
]);

  
  export default router