import React from "react";
import { Outlet } from "react-router-dom";
import HeaderLoginUser from "./HeaderLoginUser.tsx";
import Footer from "./Footer.tsx";
import { AppProvider } from "./AppContext.tsx";

const Layout = () => {
  return (
    <AppProvider>
      <div className="flex flex-col min-h-screen">
        <HeaderLoginUser />
        <main className="flex-grow p-4">
          <Outlet />
        </main>
        <Footer />
      </div>
    </AppProvider>
  );
};

export default Layout;