using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IKorisniciRepository
    {
        Task<List<Korisnici>> GetAllAsync();
        Task<Korisnici> GetByIdAsync(Guid id);
        Task<List<SelectListItem>> GetSelectOptionsAsync();
        Task<Korisnici> AddAsync(Korisnici korisnik);
        Task<Korisnici> GetByIdentityUserIdAsync(string identityUserId);
    }
}
