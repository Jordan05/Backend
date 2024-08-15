using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
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
                    // Convertir userId a ObjectId
                    var objectId = new ObjectId(userId.ToString());
                    // Adjuntar usuario al contexto en caso de validación exitosa del JWT
                    context.Items["User"] = userService.GetAsync(objectId).Result;
                }
            }
            catch
            {
                // No hacer nada si la validación del JWT falla
            }
        }
    }
}
