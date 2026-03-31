using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Services.Interfaces
{
    public interface IZanroviService
    {
        Task<ServiceResult<IEnumerable<ZanrDto>>> GetAllAsync();
    }
}
