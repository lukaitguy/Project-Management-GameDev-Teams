using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IZadaciService
    {
        Task<ServiceResult<IEnumerable<ZadatakListDto>>> GetByProjekatIdAsync(Guid projekatId);
        Task<ServiceResult<ZadatakDetailsDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<Guid>> CreateAsync(Guid projekatId, CreateZadatakDto dto, string identityUserId);
        Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateZadatakDto dto);
        Task<ServiceResult<bool>> UpdateStatusAsync(Guid id, string noviStatus, string identityUserId, bool isAdmin);
        Task<ServiceResult<bool>> DodeliZadatakAsync(Guid id, Guid korisnikId);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}
