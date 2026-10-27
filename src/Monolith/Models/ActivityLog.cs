namespace AgriculturalWebsite.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;      // Created, Updated, Deleted
        public string EntityType { get; set; } = string.Empty;  // Tractor, Trailer, Product
        public int? EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }
    }
}
