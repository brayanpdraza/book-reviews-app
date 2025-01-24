using Dominio.Servicios.Contratos;
using Dominio.Usuarios.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class ComentarioValidator : IValidate<string>
    {
        public bool Validate(string Comentario)
        {
            if (Comentario == null)
            {
                throw new ArgumentException("El comentario no puede ser nulo.");

            }
            if (string.IsNullOrEmpty(Comentario.Trim()))
            {
                throw new ArgumentException("El comentario no puede estar vacío.");
            }
            return true;
        }
    
    }
}
