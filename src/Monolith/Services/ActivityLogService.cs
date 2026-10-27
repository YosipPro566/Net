using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalWebsite.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(ApplicationDbContext context, ILogger<ActivityLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string action, string entityType, int? entityId, string entityName, string? userId, string? userEmail, string? details = null)
        {
            try
            {
                var log = new ActivityLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    EntityName = entityName,
                    UserId = userId,
                    UserEmail = userEmail,
                    Details = details,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ActivityLogs.Add(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[{Action}] {EntityType} '{EntityName}' by {UserEmail}", action, entityType, entityName, userEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при збереженні логу");
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetLogsAsync(int page = 1, int pageSize = 50)
        {
            return await _context.ActivityLogs
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetLogsByEntityAsync(string entityType)
        {
            return await _context.ActivityLogs
                .Where(l => l.EntityType == entityType)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetLogsByUserAsync(string userEmail)
        {
            return await _context.ActivityLogs
                .Where(l => l.UserEmail == userEmail)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
