using Microsoft.AspNetCore.Mvc;
using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Controllers.Api
{
    [ApiController]
    [Route("api/tractors")]
    [Produces("application/json")]
    public class TractorsApiController : ControllerBase
    {
        private readonly ITractorService _tractorService;
        public TractorsApiController(ITractorService tractorService) => _tractorService = tractorService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _tractorService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _tractorService.GetByIdAsync(id);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TractorCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var farmerId = User.Identity?.Name ?? "unknown";
            var t = await _tractorService.CreateAsync(dto, farmerId);
            return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TractorUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID не співпадає");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var t = await _tractorService.UpdateAsync(dto);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tractorService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
