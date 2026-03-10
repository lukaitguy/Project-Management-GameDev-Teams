using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IZadaciKomentarRepository
    {
        Task<KomentariZadatak> AddAsync(KomentariZadatak komentar);
        Task<IEnumerable<KomentariZadatak>> GetAllByIdAsync(Guid zadatakId);
    }
}
