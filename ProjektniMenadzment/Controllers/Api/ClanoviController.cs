using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Authorize]
    [Route("api/projekti/{projekatId}/clanovi")]
    [ApiController]
    public class ClanoviController : ControllerBase
    {
        private readonly IClanoviProjektaRepository _clanoviRepo;
        private readonly IKorisniciRepository _korisniciRepo;

        public ClanoviController(
            IClanoviProjektaRepository clanoviRepo,
            IKorisniciRepository korisniciRepo)
        {
            _clanoviRepo = clanoviRepo;
            _korisniciRepo = korisniciRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetClanovi(Guid projekatId)
        {
            var clanovi = await _clanoviRepo.GetByProjekatIdAsync(projekatId);
            var dto = clanovi.Select(c => new ClanProjekatDto
            {
                KorisnikId = c.KorisnikId,
                Ime = c.Korisnik.Ime,
                Prezime = c.Korisnik.Prezime,
                Email = c.Korisnik.Email,
                Uloga = c.Uloga
            });
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> AddClan(Guid projekatId, [FromBody] AddClanDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email je obavezan." });

            var korisnik = await _korisniciRepo.GetByEmailAsync(dto.Email);
            if (korisnik == null)
                return NotFound(new { message = "Korisnik sa tim emailom nije pronadjen." });

            var postojeci = await _clanoviRepo.GetByProjekatIdAsync(projekatId);
            if (postojeci.Any(c => c.KorisnikId == korisnik.Id))
                return Conflict(new { message = "Korisnik je vec clan projekta." });

            await _clanoviRepo.AddAsync(new ClanoviProjektum
            {
                ProjekatId = projekatId,
                KorisnikId = korisnik.Id,
                Uloga = dto.Uloga?.Trim()
            });

            return Ok(new { message = "Clan uspesno dodat." });
        }

        [HttpDelete("{korisnikId}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> RemoveClan(Guid projekatId, Guid korisnikId)
        {
            var uspesno = await _clanoviRepo.RemoveAsync(projekatId, korisnikId);
            if (!uspesno)
                return NotFound(new { message = "Clan nije pronadjen." });
            return NoContent();
        }
    }
}
