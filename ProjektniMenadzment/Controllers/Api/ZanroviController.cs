using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZanroviController : ControllerBase
    {
        private readonly IZanroviService _zanroviService;

        public ZanroviController(IZanroviService zanroviService)
        {
            _zanroviService = zanroviService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _zanroviService.GetAllAsync();

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(result.Data);
        }
    }
}
