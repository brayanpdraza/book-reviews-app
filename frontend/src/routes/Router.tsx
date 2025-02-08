import React from "react";
import { createBrowserRouter } from "react-router-dom";
import Home from './Home.tsx';
import Error from './Error.tsx';
import Login from './Login.tsx';
import Register from './Register.tsx';
import DetalleLibro from './DetalleLibro.tsx';
import ProfilePage from './ProfilePage.tsx';
import ReviewsPage from './ReviewsPage.tsx';
import Layout from '../components/Layout.tsx'
import UserLayout from '../components/UserLayout.tsx';

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
      {
        path: "/Usuario/:idUsuario",
        element: <UserLayout />,  // Layout común con navegación
        children: [
          { path: "Perfil", element: <ProfilePage /> },
          { path: "Reviews", element: <ReviewsPage /> }
        ]
      },
    ],
  },
]);

  
  export default router