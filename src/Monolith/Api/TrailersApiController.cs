using Microsoft.AspNetCore.Mvc;
using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Controllers.Api
{
    [ApiController]
    [Route("api/trailers")]
    [Produces("application/json")]
    public class TrailersApiController : ControllerBase
    {
        private readonly ITrailerService _trailerService;
        public TrailersApiController(ITrailerService trailerService) => _trailerService = trailerService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _trailerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _trailerService.GetByIdAsync(id);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrailerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var farmerId = User.Identity?.Name ?? "unknown";
            var t = await _trailerService.CreateAsync(dto, farmerId);
            return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TrailerUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID не співпадає");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var t = await _trailerService.UpdateAsync(dto);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _trailerService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
