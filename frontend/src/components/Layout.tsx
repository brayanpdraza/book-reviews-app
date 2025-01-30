import React from 'react';
import { Outlet } from 'react-router-dom';
import HeaderLoginUser from './HeaderLoginUser.tsx';
import { AppProvider } from './AppContext.tsx';

const Layout = () => {
  return (
    <AppProvider>
      <div>
        <HeaderLoginUser />
        <Outlet />
      </div>
    </AppProvider>
  );
};

export default Layout;