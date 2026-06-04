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

        Task<IEnumerable<Zadaci>> GetByKorisnikIdAsync(Guid korisnikId);
        Task<bool> DodeliZadatakAsync(Guid zadatakId, Guid korisnikId);
        Task<bool> PreuzmiAsync(Guid zadatakId, Guid korisnikId);
        Task<bool> OdustaniAsync(Guid zadatakId, Guid korisnikId);
    }
}
