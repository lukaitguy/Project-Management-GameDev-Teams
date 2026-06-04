using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IAdminKorisniciService
    {
        Task<ServiceResult<IEnumerable<AdminKorisnikListDto>>> GetAllAsync(string? search, string? role);
        Task<ServiceResult<AdminKorisnikListDto>> GetByIdAsync(string identityUserId);
        Task<ServiceResult<IEnumerable<string>>> GetRolesAsync();
        Task<ServiceResult<bool>> CreateAsync(AdminCreateKorisnikDto dto);
        Task<ServiceResult<bool>> UpdateAsync(string identityUserId, AdminUpdateKorisnikDto dto);
        Task<ServiceResult<bool>> ChangePasswordAsync(string identityUserId, AdminPromeniLozinkuDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string identityUserId);
    }
}
