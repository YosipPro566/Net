using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(string action, string entityType, int? entityId, string entityName, string? userId, string? userEmail, string? details = null);
        Task<IEnumerable<ActivityLog>> GetLogsAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<ActivityLog>> GetLogsByEntityAsync(string entityType);
        Task<IEnumerable<ActivityLog>> GetLogsByUserAsync(string userEmail);
    }
}
