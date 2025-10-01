using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
