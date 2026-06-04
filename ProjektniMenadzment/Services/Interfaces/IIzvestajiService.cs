using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IIzvestajiService
    {
        Task<ServiceResult<ProjekatIzvestajDto>> GetProjekatIzvestajAsync(Guid projekatId, string identityUserId, bool isAdmin);
    }
}
