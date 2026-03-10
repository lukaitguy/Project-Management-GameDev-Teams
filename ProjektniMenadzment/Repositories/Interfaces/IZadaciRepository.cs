using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IZadaciRepository
    {
        Task<Zadaci?> GetByIdAsync(Guid id);
        Task<IEnumerable<Zadaci>> GetByProjekatIdAsync(Guid projekatId);
        Task<Zadaci> AddAsync(Zadaci zadatak);
        Task<Zadaci> UpdateAsync(Zadaci zadatak);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid zadatakId, Guid? korisnikId, string noviStatus, bool canManageAll);

        Task<bool> DodeliZadatakAsync(Guid zadatakId, Guid korisnikId);

    }
}
