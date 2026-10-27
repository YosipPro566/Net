using Microsoft.AspNetCore.Mvc;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Controllers.Api
{
    [ApiController]
    [Route("api/logs")]
    [Produces("application/json")]
    public class LogsApiController : ControllerBase
    {
        private readonly IActivityLogService _logService;
        public LogsApiController(IActivityLogService logService) => _logService = logService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
            => Ok(await _logService.GetLogsAsync(page, pageSize));

        [HttpGet("entity/{entityType}")]
        public async Task<IActionResult> GetByEntity(string entityType)
            => Ok(await _logService.GetLogsByEntityAsync(entityType));

        [HttpGet("user/{userEmail}")]
        public async Task<IActionResult> GetByUser(string userEmail)
            => Ok(await _logService.GetLogsByUserAsync(userEmail));
    }
}
