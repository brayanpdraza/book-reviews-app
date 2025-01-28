import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import { RouterProvider } from 'react-router-dom';
import router from './routes/Router.tsx'; 
import reportWebVitals from './reportWebVitals.ts';


  const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
  );
  root.render(
    <React.StrictMode>   
      <RouterProvider router={router}/>
    </React.StrictMode>
  );

  reportWebVitals();