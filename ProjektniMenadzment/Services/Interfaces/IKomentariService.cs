using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IKomentariService
    {
        Task<ServiceResult<IEnumerable<KomentarDto>>> GetByZadatakIdAsync(Guid zadatakId);
        Task<ServiceResult<KomentarDto>> AddAsync(Guid zadatakId, CreateKomentarDto dto, string identityUserId);
    }
}
