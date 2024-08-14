using Microsoft.Extensions.Options;
using MyApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyApi.Services
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, user.Email), // Correcto: ClaimTypes.Email para el correo electrónico
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Correcto: ClaimTypes.NameIdentifier para el ID del usuario
        }),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        Issuer = _jwtSettings.Issuer,
        Audience = _jwtSettings.Audience
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

    }
}
