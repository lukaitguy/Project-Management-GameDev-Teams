using ProjektniMenadzment.Models.DTOs;
using System.Security.Claims;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult<ProfilDto>> GetProfilAsync(ClaimsPrincipal user);
        Task<ServiceResult<object>> UpdateProfilAsync(ClaimsPrincipal user, UpdateProfilDto dto);
        Task<ServiceResult<object>> PromeniKorisnickoImeAsync(ClaimsPrincipal user, PromeniKorisnickoImeDto dto);
        Task<ServiceResult<object>> PromeniLozinkuAsync(ClaimsPrincipal user, PromeniLozinkuDto dto);
    }
}
