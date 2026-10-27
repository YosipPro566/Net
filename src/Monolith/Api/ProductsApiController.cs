using Microsoft.AspNetCore.Mvc;
using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Controllers.Api
{
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsApiController(IProductService productService) => _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _productService.GetByIdAsync(id);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var farmerId = User.Identity?.Name ?? "unknown";
            var p = await _productService.CreateAsync(dto, farmerId);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID не співпадає");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var p = await _productService.UpdateAsync(dto);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
