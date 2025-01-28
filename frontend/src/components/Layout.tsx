import React from "react";
import { Outlet } from "react-router-dom";
import HeaderLoginUser from "./HeaderLoginUser.tsx";

const Layout = () => {
  return (
    <div>
      <HeaderLoginUser /> {/* Aquí se incluye el Header en todas las páginas */}
      <Outlet /> {/* Aquí se renderiza el contenido de cada ruta */}
    </div>
  );
};

export default Layout;