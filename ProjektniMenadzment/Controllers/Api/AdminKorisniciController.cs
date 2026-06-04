using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/admin/korisnici")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdminKorisniciController : ControllerBase
    {
        private readonly IAdminKorisniciService _service;

        public AdminKorisniciController(IAdminKorisniciService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? role)
        {
            var result = await _service.GetAllAsync(search, role);
            return result.Success ? Ok(result.Data) : BadRequest(new { message = result.Message });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _service.GetRolesAsync();
            return result.Success ? Ok(result.Data) : BadRequest(new { message = result.Message });
        }

        [HttpGet("{identityUserId}")]
        public async Task<IActionResult> GetById(string identityUserId)
        {
            var result = await _service.GetByIdAsync(identityUserId);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateKorisnikDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok();
        }

        [HttpPut("{identityUserId}")]
        public async Task<IActionResult> Update(string identityUserId, [FromBody] AdminUpdateKorisnikDto dto)
        {
            var result = await _service.UpdateAsync(identityUserId, dto);
            if (!result.Success)
                return result.Message == "Korisnik nije pronadjen."
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
            return NoContent();
        }

        [HttpPut("{identityUserId}/lozinka")]
        public async Task<IActionResult> ChangePassword(string identityUserId, [FromBody] AdminPromeniLozinkuDto dto)
        {
            var result = await _service.ChangePasswordAsync(identityUserId, dto);
            if (!result.Success)
                return result.Message == "Korisnik nije pronadjen."
                    ? NotFound(new { message = result.Message })
                    : BadRequest(new { message = result.Message });
            return NoContent();
        }

        [HttpDelete("{identityUserId}")]
        public async Task<IActionResult> Delete(string identityUserId)
        {
            var result = await _service.DeleteAsync(identityUserId);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return NoContent();
        }
    }
}
