using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IResursiService
    {
        Task<ServiceResult<IEnumerable<ResursListDto>>> GetByProjekatIdAsync(Guid projekatId);
        Task<ServiceResult<IEnumerable<MojResursDto>>> GetMojiResursiAsync(string identityUserId);
        Task<ServiceResult<ResursListDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<Guid>> CreateAsync(Guid projekatId, CreateResursDto dto);
        Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateResursDto dto);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}
