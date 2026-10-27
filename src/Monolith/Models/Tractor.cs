namespace AgriculturalWebsite.Models
{
    public class Tractor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Power { get; set; } // Мощность в л.с.
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string? FarmerId { get; set; } // ID фермера-владельца
    }
}
