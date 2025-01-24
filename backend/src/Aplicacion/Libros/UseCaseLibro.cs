using Dominio.Entidades.Libros.Puertos;
using Dominio.Libros.Modelo;
using Dominio.Servicios.ServicioPaginacion.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Libros
{   
    public class UseCaseLibro
    {
        private readonly ILibroRepositorio _libroRepositorio;

        public UseCaseLibro(ILibroRepositorio libroRepositorio)
        {
            _libroRepositorio = libroRepositorio;
        }

        public PaginacionResultadoModelo<LibroModelo> ConsultarLibrosPaginados(int pagina, int tamanoPagina, string filtro = null)
        {
            List<LibroModelo> libros;
            int totalRegistros, skip;

            if (pagina <= 0 || tamanoPagina <= 0)
            {
                throw new ArgumentException("La página y el tamaño de página deben ser mayores a cero.");
            }

            totalRegistros = _libroRepositorio.ConteoLibros(filtro);

            skip = (pagina - 1) * tamanoPagina;

            libros = _libroRepositorio.ListLibrosPaginadosPorFiltroOpcional(skip, tamanoPagina, filtro);

            return new PaginacionResultadoModelo<LibroModelo>
            {
                Items = libros,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public LibroModelo ConsultarLibroPorId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Debe ingresar un ID Válido para consultar un libro");
            }

            return _libroRepositorio.ListLibroPorId(id);

        }
    }
}
