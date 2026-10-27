namespace AgriculturalWebsite.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Категория продукта
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty; // Единица измерения (кг, л, шт)
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string? FarmerId { get; set; }
    }
}
