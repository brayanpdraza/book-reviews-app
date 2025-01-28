import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { fetchConfig } from '../methods/fetchConfig.ts';
import {Libro} from '../Interfaces/Libro.ts';

interface ApiResponse<T> {
  items: T[];
  totalRegistros: number;
  paginaActual: number;
  tamanoPagina: number;
  totalPaginas: number;s
}

interface PaginacionLibrosProps {
  itemsPorPagina: number;
}

const PaginacionLibros: React.FC<PaginacionLibrosProps> = ({ itemsPorPagina }) => {
  const [libros, setLibros] = useState<Libro[]>([]);
  const [paginaActual, setPaginaActual] = useState(1);
  const location = useLocation();
  const [tipoFiltroTemp, setTipoFiltroTemp] = useState<'categoria' | 'titulo' | 'autor'>('titulo'); // Tipo de filtro
  const [tipoFiltro, setTipoFiltro] = useState<'categoria' | 'titulo' | 'autor'>('titulo'); // Tipo de filtro
  const [inputValue, setInputValue] = useState(''); // Valor temporal del input
  const [valorFiltro, setValorFiltro] = useState(''); // Valor del filtro
  const [totalLibros, setTotalLibros] = useState(0);
  const [totalPaginas, setTotalPaginas] = useState(0);
  const [apiUrl, setAPIUrl] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loadingConfig, setLoadingConfig] = useState(true);
  const navigate = useNavigate();
  const ControllerName = 'Libro';

 const fetchLibros = async (pagina: number, filtro?: string) => {
  try {
    let url = `${apiUrl}/${ControllerName}/ConsultarLibrosPaginadosFiltroOpcional/${pagina}/${itemsPorPagina}`;
    if (filtro!== undefined && filtro !== null && filtro !== "") {
      url += `?filtro=${filtro}`; // Agregar el filtro solo si existe
    }

    const response = await fetch(url);
    if (!response.ok) {
      setError(`Error 23523: ${error}`);
      console.error('Error fetching libros:', error);
      return;
    }

    const data: ApiResponse<Libro> = await response.json();
    setLibros(data.items);
    setTotalPaginas(data.totalPaginas);
    setTotalLibros(data.totalRegistros);
  } catch (error) {
    setError(`Error 6436: ${error}`);
    console.error('Error fetching libros:', error);
  }
};

useEffect(() => {
  const loadConfig = async () => {
  await fetchConfig(setAPIUrl, setError, setLoadingConfig);
  };
  loadConfig();
}, [location.pathname]);

// Cargar libros al cambiar de página o al aplicar un filtro
useEffect(() => {
  if (!apiUrl) {
    return
  }
  const filtroCompleto = valorFiltro ? `${tipoFiltro}:${valorFiltro}` : "";
  fetchLibros(paginaActual, filtroCompleto);
}, [apiUrl,paginaActual, valorFiltro,tipoFiltro]);

// Redirigir al detalle del libro
const handleClickLibro = (id: number) => {
  navigate(`/detallelibro/${id}`);
};

  // Manejar el cambio en el tipo de filtro temporal
  const handleTipoFiltroChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    setTipoFiltroTemp(event.target.value as 'categoria' | 'titulo' | 'autor');
  };

  // Manejar el cambio en el valor temporal del input
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };


// Aplicar el filtro (se ejecuta al presionar "Enter" o al hacer clic en el botón)
const aplicarFiltro = () => {
  setPaginaActual(1); // Reiniciar a la primera página al aplicar un filtro
  setValorFiltro(inputValue);
  setTipoFiltro(tipoFiltroTemp);
};

if (loadingConfig) {
  return <div>Cargando configuración...</div>;
}

if (error) {
  return <div>{error}</div>;
}
return (
  <div className="p-6 bg-gray-50 min-h-screen">
    {/* Filtro dinámico */}
    <div className="flex items-center gap-4 mb-8">
      <select
        value={tipoFiltroTemp}
        onChange={handleTipoFiltroChange}
        className="p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        <option value="categoria">Categoría</option>
        <option value="titulo">Título</option>
        <option value="autor">Autor</option>
      </select>
      <input
        type="text"
        placeholder={`Filtrar por ${tipoFiltroTemp}...`}
        value={inputValue}
        onChange={handleInputChange}
        onKeyPress={(e) => {
          if (e.key === 'Enter') aplicarFiltro();
        }}
        className="p-2 border border-gray-300 rounded-md flex-grow focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
      <button
        onClick={aplicarFiltro}
        className="p-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        Filtrar
      </button>
    </div>

    {/* Lista de libros */}
    <ul className="space-y-4">
      {libros.map((libro) => (
        <li
          key={libro.id}
          onClick={() => handleClickLibro(libro.id)}
          className="p-4 bg-white shadow-md rounded-lg cursor-pointer hover:shadow-lg transition-shadow"
        >
          <h3 className="text-xl font-semibold text-gray-800">{libro.titulo}</h3>
          <p className="text-gray-600">Autor: {libro.autor} | Categoria: {libro.categoria}</p> 
        </li>
      ))}
    </ul>

    {/* Paginación */}
    <div className="flex justify-center gap-2 mt-8">
      {Array.from({ length: totalPaginas }, (_, i) => (
        <button
          key={i + 1}
          onClick={() => setPaginaActual(i + 1)}
          className={`px-4 py-2 border rounded-md ${
            paginaActual === i + 1
              ? "bg-blue-500 text-white border-blue-500"
              : "bg-white text-gray-700 border-gray-300 hover:bg-gray-100"
          } focus:outline-none focus:ring-2 focus:ring-blue-500`}
        >
          {i + 1}
        </button>
      ))}
    </div>
  </div>
);
};
export default PaginacionLibros;