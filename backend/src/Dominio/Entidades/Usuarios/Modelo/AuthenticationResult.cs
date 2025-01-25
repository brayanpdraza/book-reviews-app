using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Usuarios.Modelo
{
    public class AuthenticationResult
    {
        public string Credential { get; set; }
        public string RenewalCredential { get; set; } 
        public DateTime Expiry { get; set; }
    }
}
