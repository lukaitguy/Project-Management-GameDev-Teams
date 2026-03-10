using ProjektniMenadzment.Models.ViewModels;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IAdminKorisniciRepository
    {
        Task<List<KorisniciViewModel>> GetAllAsync(string? search = null, string? role = null, bool onlyLinked = false);
        Task<KorisniciViewModel?> GetByIdentityIdAsync(string identityUserId);
        Task<List<string>> GetAllRolesAsync();

        Task UpdateAsync(KorisniciViewModel korisnik);
        Task CreateAppUserAsync(KorisniciViewModel korisnik);
        Task DeleteAsync(string identityUserId);
        Task ChangePasswordAsync(PromenaLozinkeViewModel lozinka);
        Task CreateAsync(KreirajKorisnikaViewModel korisnik);
    }
}
