using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/izvestaji")]
    [ApiController]
    [Authorize(Roles = "Administrator,ProjektniMenadzer")]
    public class IzvestajiController : ControllerBase
    {
        private readonly IIzvestajiService _service;
        private readonly UserManager<IdentityUser> _userManager;

        public IzvestajiController(IIzvestajiService service, UserManager<IdentityUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet("projekti/{projekatId}")]
        public async Task<IActionResult> GetProjekatIzvestaj(Guid projekatId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var isAdmin = User.IsInRole("Administrator");
            var result = await _service.GetProjekatIzvestajAsync(projekatId, identityUser.Id, isAdmin);

            if (!result.Success)
            {
                return result.Message!.Contains("pristup")
                    ? Forbid()
                    : NotFound(new { message = result.Message });
            }

            return Ok(result.Data);
        }
    }
}
