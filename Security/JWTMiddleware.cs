using Microsoft.AspNetCore.Http;
using MyApi.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Security
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserService userService, ITokenService tokenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, userService, tokenService, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, UserService userService, ITokenService tokenService, string token)
        {
            try
            {
                var userId = tokenService.ValidateToken(token);
                if (userId != null)
                {
                    // Attach user to context on successful jwt validation
                    context.Items["User"] = userService.GetAsync(userId.Value).Result;
                }
            }
            catch
            {
                // Do nothing if jwt validation fails
            }
        }
    }
}
