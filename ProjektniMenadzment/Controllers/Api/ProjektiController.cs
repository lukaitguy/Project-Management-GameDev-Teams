using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
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

            if(identityUser == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var result = await _projektiService.GetMojiProjektiAsync(identityUser.Id);

            if (!result.Success)
            {
                return NotFound(new { message = result.Message });
            }
            return Ok(result.Data);
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjekatById(Guid id)
        {
            var projekat = _projektiService.GetByIdAsync(id);

            if (!projekat.Result.Success)
            {
                return NotFound(new { message = "Projekat nije pronadjen." });
            }

            return Ok(projekat.Result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateProjekatDto projekat)
        {
            var result = await _projektiService.CreateAsync(projekat);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { id = result.Data });
             
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjekatDto projekat)
        {
            throw new NotImplementedException();
        }

    }
}
