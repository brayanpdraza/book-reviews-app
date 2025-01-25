using AdaptadorAPI.Contratos;
using System.IdentityModel.Tokens.Jwt;

namespace AdaptadorAPI.JWT.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenBlacklist _tokenBlacklist;

        public JwtMiddleware(RequestDelegate next, ITokenBlacklist tokenBlacklist)
        {
            _next = next;
            _tokenBlacklist = tokenBlacklist;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (_tokenBlacklist.IsTokenBlacklisted(jwtToken.Id))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // Adjuntar usuario al contexto si es necesario
                context.Items["User"] = jwtToken;
            }

            await _next(context);
        }
    }
}
