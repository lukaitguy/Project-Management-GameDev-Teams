using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Authorize]
    [Route("api/projekti/{projekatId}/zadaci/{zadatakId}/komentari")]
    [ApiController]
    public class KomentariController : ControllerBase
    {
        private readonly IKomentariService _komentariService;
        private readonly UserManager<IdentityUser> _userManager;

        public KomentariController(
            IKomentariService komentariService,
            UserManager<IdentityUser> userManager)
        {
            _komentariService = komentariService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetByZadatak(Guid zadatakId)
        {
            var result = await _komentariService.GetByZadatakIdAsync(zadatakId);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Add(
            Guid projekatId, Guid zadatakId, [FromBody] CreateKomentarDto dto)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _komentariService.AddAsync(zadatakId, dto, identityUser.Id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}
