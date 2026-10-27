namespace AgriculturalWebsite.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
    }
}