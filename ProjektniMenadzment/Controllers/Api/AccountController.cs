using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfil()
        {
            var result = await _accountService.GetProfilAsync(User);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfil([FromBody] UpdateProfilDto dto)
        {
            var result = await _accountService.UpdateProfilAsync(User, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("korisnicko-ime")]
        public async Task<IActionResult> PromeniKorisnickoIme([FromBody] PromeniKorisnickoImeDto dto)
        {
            var result = await _accountService.PromeniKorisnickoImeAsync(User, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("lozinka")]
        public async Task<IActionResult> PromeniLozinku([FromBody] PromeniLozinkuDto dto)
        {
            var result = await _accountService.PromeniLozinkuAsync(User, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
