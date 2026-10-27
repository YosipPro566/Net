using System.ComponentModel.DataAnnotations;

namespace AgriculturalWebsite.DTOs
{
    public class TrailerCreateDto
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Тип обов'язковий")]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вантажопідйомність обов'язкова")]
        [Range(1, 100000, ErrorMessage = "Вантажопідйомність повинна бути від 1 до 100000 кг")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Url]
        public string? ImageUrl { get; set; }
    }

    public class TrailerUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Тип обов'язковий")]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вантажопідйомність обов'язкова")]
        [Range(1, 100000)]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Url]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class TrailerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string? FarmerId { get; set; }
    }
}
