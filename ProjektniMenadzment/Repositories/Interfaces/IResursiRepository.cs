using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IResursiRepository
    {
        Task<Resursi> AddAsync(Resursi resurs);
        Task<Resursi?> UpdateAsync(Resursi resurs);
        Task<bool> DeleteAsync(Guid id);
        Task<Resursi?> GetByIdAsync(Guid id);
        Task<IEnumerable<Resursi>> GetAllAsync();
        Task<IEnumerable<Resursi>> GetByProjekatIdAsync(Guid projekatId);
        Task<IEnumerable<Resursi>> GetByKorisnikIdAsync(Guid korisnikId);
    }
}
