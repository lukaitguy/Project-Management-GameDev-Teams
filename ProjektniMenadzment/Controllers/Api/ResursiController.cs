using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/projekti/{projekatId}/resursi")]
    [ApiController]
    public class ResursiController : ControllerBase
    {
        private readonly IResursiService _resursiService;

        public ResursiController(IResursiService resursiService)
        {
            _resursiService = resursiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByProjekat(Guid projekatId)
        {
            var result = await _resursiService.GetByProjekatIdAsync(projekatId);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid projekatId, Guid id)
        {
            var result = await _resursiService.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { message = result.Message });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Create(Guid projekatId, [FromBody] CreateResursDto dto)
        {
            var result = await _resursiService.CreateAsync(projekatId, dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { id = result.Data });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Update(Guid projekatId, Guid id, [FromBody] UpdateResursDto dto)
        {
            var result = await _resursiService.UpdateAsync(id, dto);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,ProjektniMenadzer")]
        public async Task<IActionResult> Delete(Guid projekatId, Guid id)
        {
            var result = await _resursiService.DeleteAsync(id);
            if (!result.Success)
                return HandleFailure(result.Message);
            return NoContent();
        }

        private IActionResult HandleFailure(string? message) =>
            string.Equals(message, "Resurs nije pronadjen.", StringComparison.Ordinal)
                ? NotFound(new { message })
                : BadRequest(new { message = message ?? "Došlo je do greške." });
    }
}
