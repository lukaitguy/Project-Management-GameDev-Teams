using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IProjektiService
    {
        Task<ServiceResult<Guid>> CreateAsync(CreateProjekatDto dto, string identityUserId);
        Task<ServiceResult<IEnumerable<ProjekatListDto>>> GetAllAsync();
        Task<ServiceResult<IEnumerable<ProjekatListDto>>> GetMojiProjektiAsync(string identityUserId);
        Task<ServiceResult<ProjekatDetailsDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateProjekatDto dto);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}
