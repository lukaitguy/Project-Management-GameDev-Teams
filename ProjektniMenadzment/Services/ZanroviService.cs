using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class ZanroviService : IZanroviService
    {
        private readonly IZanroviRepository _zanroviRepository;

        public ZanroviService(IZanroviRepository zanroviRepository)
        {
            _zanroviRepository = zanroviRepository;
        }

        public async Task<ServiceResult<IEnumerable<ZanrDto>>> GetAllAsync()
        {
            try
            {
                var zanrovi = await _zanroviRepository.GetAllAsync();

                var result = zanrovi.Select(z => new ZanrDto
                {
                    Id = z.Id,
                    Naziv = z.Naziv,
                });

                return ServiceResult<IEnumerable<ZanrDto>>.Ok(result);
            }
            catch
            {
                return ServiceResult<IEnumerable<ZanrDto>>.Fail("Greska pri ucitavanju zanrova.");
            }
        }
    }
}
