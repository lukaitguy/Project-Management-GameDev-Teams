using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IBuildoviRepository
    {
        Task<List<Buildovi>> GetAllAsync();
        Task<Buildovi?> GetByIdAsync(Guid id);
        Task<List<Buildovi>> GetByProjekatIdAsync(Guid projekatId);
        Task<Buildovi> CreateAsync(Buildovi build);
        Task<Buildovi?> UpdateAsync(Buildovi build);
        Task<Buildovi?> DeleteAsync(Guid id);
    }
}
