using EBZShared.Models;

namespace EBZWebApi.Services
{
    public interface IJWTAuthService
    {
        string GenerateToken(User user);
    }
}