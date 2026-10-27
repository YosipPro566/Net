using System.ComponentModel.DataAnnotations;

namespace AgriculturalWebsite.DTOs
{
    public class TractorCreateDto
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Марка обов'язкова")]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Модель обов'язкова")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потужність обов'язкова")]
        [Range(1, 1000, ErrorMessage = "Потужність повинна бути від 1 до 1000 к.с.")]
        public int Power { get; set; }

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Опис обов'язковий")]
        [StringLength(1000, ErrorMessage = "Опис не може перевищувати 1000 символів")]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "Невірний формат URL")]
        public string? ImageUrl { get; set; }
    }

    public class TractorUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Марка обов'язкова")]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Модель обов'язкова")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потужність обов'язкова")]
        [Range(1, 1000)]
        public int Power { get; set; }

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

    public class TractorResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Power { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string? FarmerId { get; set; }
    }
}
