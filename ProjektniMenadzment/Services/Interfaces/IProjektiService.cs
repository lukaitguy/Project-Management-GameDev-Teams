using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IProjektiService
    {
        Task<ServiceResult<Guid>> CreateAsync(CreateProjekatDto dto);
        Task<ServiceResult<IEnumerable<ProjekatListDto>>> GetMojiProjektiAsync(string identityUserId);
        Task<ServiceResult<ProjekatDetailsDto>> GetByIdAsync(Guid id);
        Task UpdateAsync(UpdateProjekatDto dto);
    }
}
