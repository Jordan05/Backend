using MyApi.Models; // Asegúrate de que este espacio de nombres sea correcto

namespace MyApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user); // Asegúrate de que 'User' esté definido en MyApi.Models
        int? ValidateToken(string token);
    }
}
