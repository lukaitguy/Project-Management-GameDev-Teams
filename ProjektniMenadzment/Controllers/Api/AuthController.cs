using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registracija")]
        public async Task<IActionResult> Registracija([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegistracijaAsync(request);

            if(result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("prijava")]
        public async Task<IActionResult> Prijava([FromBody] LoginRequestDto request)
        {
            var result = await _authService.PrijavaAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


    }
}