import React from 'react';
import PaginacionLibros from '../components/ListaPaginacionLibros.tsx'

export default function Home() {
    return (
      <div className="mainContainer">
        <div className="content-wrapper">
          {/* Contenido principal */}
          <div className="content p-4">
            <PaginacionLibros itemsPorPagina={20} />
          </div>
        </div>
      </div>
    );
  }