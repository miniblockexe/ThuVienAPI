using Microsoft.AspNetCore.Identity;

namespace LTW2.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
