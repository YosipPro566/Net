using System.ComponentModel.DataAnnotations;

namespace AgriculturalWebsite.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Категорія обов'язкова")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Одиниця виміру обов'язкова")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Url]
        public string? ImageUrl { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Категорія обов'язкова")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Одиниця виміру обов'язкова")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Url]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string? FarmerId { get; set; }
    }
}
