using Microsoft.IdentityModel.Tokens;

namespace AdaptadorAPI.Servicios
{
    public class ObtenerDatosUsuarioToken
    {
        public long ObtenerIdUsuario(HttpContext Context)
        {
            long userId;
            // Obtener el usuario actual (ClaimsPrincipal)
            var usuario = Context.User;

            // Extraer el claim "id" (usuarioId)
            var usuarioId = usuario.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioId))
            {
                throw new SecurityTokenExpiredException("Claim 'id' no encontrado en el token.");
            }


            if (!long.TryParse(usuarioId, out userId))
            {
                // Usa el ID convertido (userId)
                throw new ArgumentException("No se pudo obtener un ID válido del token.");
            }

            return userId;
        }
    }
}
