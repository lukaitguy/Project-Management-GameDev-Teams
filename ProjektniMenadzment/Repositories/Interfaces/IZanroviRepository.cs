using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IZanroviRepository
    {
        Task<IEnumerable<Zanrovi>> GetAllAsync();
        Task<List<Zanrovi>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
