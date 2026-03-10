using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IZanroviRepository
    {
        Task<IEnumerable<Zanrovi>> GetAllAsync(); 
    }
}
