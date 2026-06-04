using Microsoft.AspNetCore.Identity;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(IdentityUser user, IList<string> roles, Guid? korisnikId = null);
    }
}
