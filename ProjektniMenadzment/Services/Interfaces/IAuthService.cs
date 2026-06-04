using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegistracijaAsync(RegisterRequestDto request);

        Task<ServiceResult<object>> PrijavaAsync(LoginRequestDto request);
    }
}
