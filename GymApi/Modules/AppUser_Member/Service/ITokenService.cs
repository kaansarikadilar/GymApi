using GymApi.Models;

namespace GymApi.Service
{
    public interface ITokenService
    {
        string CreateToken(AppUser user,string role);
    }
}