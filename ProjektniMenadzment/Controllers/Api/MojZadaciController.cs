using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/zadaci")]
    [ApiController]
    [Authorize]
    public class MojZadaciController : ControllerBase
    {
        private readonly IZadaciService _zadaciService;
        private readonly UserManager<IdentityUser> _userManager;

        public MojZadaciController(IZadaciService zadaciService, UserManager<IdentityUser> userManager)
        {
            _zadaciService = zadaciService;
            _userManager = userManager;
        }

        [HttpGet("moji")]
        public async Task<IActionResult> GetMojiZadaci()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _zadaciService.GetMojiZadaciAsync(identityUser.Id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }
    }
}
