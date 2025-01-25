using Dominio.Libros.Modelo;
using Dominio.Reviews.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Libros.Puertos
{
    public interface ILibroRepositorio
    {
        List<LibroModelo> ListLibrosPaginadosPorFiltroOpcional(int skip, int tamanoPagina, string filtro = null);
        LibroModelo ListLibroPorId(long id);
        int ConteoLibros(string filtro = null);
        void SaveChanges();
    }
}
