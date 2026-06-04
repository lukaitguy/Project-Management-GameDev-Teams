using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/projekti/{projekatId}/zadaci")]
    [ApiController]
    public class ZadaciController : ControllerBase
    {
        private readonly IZadaciService _zadaciService;
        private readonly UserManager<IdentityUser> _userManager;

        public ZadaciController(IZadaciService zadaciService, UserManager<IdentityUser> userManager)
        {
            _zadaciService = zadaciService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetByProjekat(Guid projekatId)
        {
            var result = await _zadaciService.GetByProjekatIdAsync(projekatId);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid projekatId, Guid id)
        {
            var result = await _zadaciService.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Create(Guid projekatId, [FromBody] CreateZadatakDto dto)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _zadaciService.CreateAsync(projekatId, dto, identityUser.Id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetById),
                new { projekatId, id = result.Data },
                new { id = result.Data });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Update(Guid projekatId, Guid id, [FromBody] UpdateZadatakDto dto)
        {
            var result = await _zadaciService.UpdateAsync(id, dto);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        // PATCH: any authenticated member can update status (repo enforces ownership)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid projekatId, Guid id, [FromBody] UpdateZadatakStatusDto dto)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var isAdmin = User.IsInRole("Administrator") || User.IsInRole("ProjektniMenadzer");
            var result = await _zadaciService.UpdateStatusAsync(id, dto.NoviStatus, identityUser.Id, isAdmin);

            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        [HttpPatch("{id}/preuzmi")]
        [Authorize]
        public async Task<IActionResult> Preuzmi(Guid projekatId, Guid id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _zadaciService.PreuzmiZadatakAsync(id, identityUser.Id);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        [HttpPatch("{id}/odustani")]
        [Authorize]
        public async Task<IActionResult> Odustani(Guid projekatId, Guid id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _zadaciService.OdustaniOdZadatkaAsync(id, identityUser.Id);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        [HttpPatch("{id}/dodeli")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Dodeli(Guid projekatId, Guid id, [FromBody] Guid korisnikId)
        {
            var result = await _zadaciService.DodeliZadatakAsync(id, korisnikId);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Delete(Guid projekatId, Guid id)
        {
            var result = await _zadaciService.DeleteAsync(id);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        private IActionResult HandleFailure(string? message) =>
            string.Equals(message, "Zadatak nije pronadjen.", StringComparison.Ordinal)
                ? NotFound(new { message })
                : BadRequest(new { message = message ?? "Doslo je do greske." });
    }
}
