using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BuildoviController : ControllerBase
    {
        private readonly IBuildoviRepository _buildoviRepository;

        public BuildoviController(IBuildoviRepository buildoviRepository)
        {
            _buildoviRepository = buildoviRepository;
        }

        [HttpGet("projekat/{projekatId}")]
        public async Task<IActionResult> GetByProjekatId(Guid projekatId)
        {
            var buildovi = await _buildoviRepository.GetByProjekatIdAsync(projekatId);

            var rezultat = buildovi.Select(b => new BuildListDto
            {
                Id = b.Id,
                Verzija = b.Verzija,
                NazivBuilda = b.NazivBuilda,
                TipBuilda = b.TipBuilda,
                PatchNapomene = b.PatchNapomene,
                DatumBuilda = b.DatumBuilda,
                ProjekatId = b.ProjekatId
            }).ToList();

            return Ok(rezultat);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBuildDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Verzija))
            {
                return BadRequest(new { message = "Verzija builda je obavezna." });
            }

            var build = new Models.Domain.Buildovi
            {
                Id = Guid.NewGuid(),
                Verzija = dto.Verzija,
                NazivBuilda = dto.NazivBuilda,
                TipBuilda = dto.TipBuilda,
                PatchNapomene = dto.PatchNapomene,
                DatumBuilda = DateTime.UtcNow,
                ProjekatId = dto.ProjekatId
            };

            var rezultat = await _buildoviRepository.CreateAsync(build);

            return Ok(new
            {
                message = "Build je uspešno dodat.",
                id = rezultat.Id
            });
        }
    }
}
