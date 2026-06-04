using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjektiController : ControllerBase
    {
        private readonly IProjektiService _projektiService;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjektiController(
            IProjektiService projektiService,
            UserManager<IdentityUser> userManager)
        {
            _projektiService = projektiService;
            _userManager = userManager;
        }

        [HttpGet("moji")]
        public async Task<IActionResult> GetMojiProjekti()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if (identityUser == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var result = User.IsInRole("Administrator")
                ? await _projektiService.GetAllAsync()
                : await _projektiService.GetMojiProjektiAsync(identityUser.Id);

            if (!result.Success)
            {
                return NotFound(new { message = result.Message });
            }

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjekatById(Guid id)
        {
            var projekat = await _projektiService.GetByIdAsync(id);

            if (!projekat.Success)
            {
                return NotFound(new { message = "Projekat nije pronadjen." });
            }

            return Ok(projekat.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Create([FromBody] CreateProjekatDto projekat)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if (identityUser == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var result = await _projektiService.CreateAsync(projekat, identityUser.Id);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { id = result.Data });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjekatDto projekat)
        {
            var result = await _projektiService.UpdateAsync(id, projekat);

            if (!result.Success)
            {
                return HandleProjectMutationFailure(result.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _projektiService.DeleteAsync(id);

            if (!result.Success)
            {
                return HandleProjectMutationFailure(result.Message);
            }

            return NoContent();
        }

        private IActionResult HandleProjectMutationFailure(string? message)
        {
            if (string.Equals(message, "Projekat nije pronadjen.", StringComparison.Ordinal))
            {
                return NotFound(new { message });
            }

            return BadRequest(new { message = message ?? "Doslo je do greske pri obradi projekta." });
        }
    }
}
