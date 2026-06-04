using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/resursi")]
    [ApiController]
    [Authorize]
    public class MojResursiController : ControllerBase
    {
        private readonly IResursiService _resursiService;
        private readonly UserManager<IdentityUser> _userManager;

        public MojResursiController(IResursiService resursiService, UserManager<IdentityUser> userManager)
        {
            _resursiService = resursiService;
            _userManager = userManager;
        }

        [HttpGet("moji")]
        public async Task<IActionResult> GetMojiResursi()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var result = await _resursiService.GetMojiResursiAsync(identityUser.Id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }
    }
}
