using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IClanoviProjektaRepository
    {
        Task<ClanoviProjektum> AddAsync(ClanoviProjektum clan);
        Task<List<ClanoviProjektum>> GetByProjekatIdAsync(Guid projekatId);
        Task<List<Korisnici>> GetKorisniciByProjekatIdAsync(Guid projekatId);
        Task<bool> RemoveAsync(Guid projekatId, Guid korisnikId);
    }
}
