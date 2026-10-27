namespace AgriculturalWebsite.Models
{
    public class Trailer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Тип прицепа
        public int Capacity { get; set; } // Грузоподъемность в кг
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string? FarmerId { get; set; }
    }
}
