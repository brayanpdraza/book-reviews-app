using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioEncripcion.Contratos
{
    public interface IEncription
    {
        string Encriptar(string password);
        bool VerificarClaveEncriptada(string hashedPassword, string providedPassword);
    }
}
