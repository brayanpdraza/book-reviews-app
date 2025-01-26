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
            int totalRegistros, skip, totalPaginas;

            if (pagina <= 0)
            {
                throw new ArgumentException("La página debe ser mayor a cero.");
            }

            if (tamanoPagina <= 0)
            {
                throw new ArgumentException("El tamaño de página debe ser mayor a cero.");
            }

            totalRegistros = _libroRepositorio.ConteoLibros(filtro);

            if (totalRegistros <= 0)
            {
                return new PaginacionResultadoModelo<LibroModelo>
                {
                    Items = new List<LibroModelo>(), // Lista vacía
                    TotalRegistros = 0,
                    PaginaActual = 1,
                    TamanoPagina = tamanoPagina
                };
            }

            totalPaginas = TotalPaginas(totalRegistros,tamanoPagina);

            if (pagina > totalPaginas)
            {
                throw new ArgumentException($"La página solicitada ({pagina}) excede el total de páginas disponibles.");
            }

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

        public LibroModelo ConsultarLibroPorId(long id)
        {
            LibroModelo Libro;
            if (id <= 0)
            {
                throw new ArgumentException("Debe ingresar un ID Válido para consultar un libro.");
            }
            Libro = _libroRepositorio.ListLibroPorId(id);

            if (Libro.Id <= 0)
            {
                throw new KeyNotFoundException($"El id {id} No se encuentra asociado a ningún libro.");
            }

            return Libro;

        }

        public int TotalPaginas(int totalRegistros, int tamanoPagina)
        {
            if(tamanoPagina == 0)
            {
                throw new DivideByZeroException("El tamano de Pagina no peude ser 0");
            }

            return (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
        }
    }
}
