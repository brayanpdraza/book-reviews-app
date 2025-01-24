using Dominio.Reviews.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios.Modelo
{
    public class UsuarioModelo
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public ICollection<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
}
