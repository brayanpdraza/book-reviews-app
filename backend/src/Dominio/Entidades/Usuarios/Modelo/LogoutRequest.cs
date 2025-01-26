using System.ComponentModel.DataAnnotations;

namespace Dominio.Entidades.Usuarios.Modelo
{
    public class LogoutRequest
    {
        [Required]
        public string Credential { get; set; }

    }
}
