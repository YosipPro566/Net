using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Repositories.Interfaces;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Отримання всіх продуктів");
            var products = await _repository.GetAvailableAsync();
            return products.Select(MapToResponseDto);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Отримання продукту з ID: {Id}", id);
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Продукт з ID {Id} не знайдено", id);
                return null;
            }
            return MapToResponseDto(product);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, string farmerId)
        {
            _logger.LogInformation("Створення нового продукту фермером {FarmerId}", farmerId);
            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price,
                Unit = dto.Unit,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                FarmerId = farmerId,
                IsAvailable = true
            };
            var created = await _repository.CreateAsync(product);
            _logger.LogInformation("Продукт створено з ID: {Id}", created.Id);
            return MapToResponseDto(created);
        }

        public async Task<ProductResponseDto?> UpdateAsync(ProductUpdateDto dto)
        {
            _logger.LogInformation("Оновлення продукту з ID: {Id}", dto.Id);
            var product = await _repository.GetByIdAsync(dto.Id);
            if (product == null)
            {
                _logger.LogWarning("Продукт з ID {Id} не знайдено для оновлення", dto.Id);
                return null;
            }
            product.Name = dto.Name;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.Unit = dto.Unit;
            product.Description = dto.Description;
            product.ImageUrl = dto.ImageUrl ?? string.Empty;
            product.IsAvailable = dto.IsAvailable;

            var updated = await _repository.UpdateAsync(product);
            _logger.LogInformation("Продукт з ID {Id} оновлено", dto.Id);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Видалення продукту з ID: {Id}", id);
            var result = await _repository.DeleteAsync(id);
            if (!result) _logger.LogWarning("Продукт з ID {Id} не знайдено для видалення", id);
            return result;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetByFarmerIdAsync(string farmerId)
        {
            _logger.LogInformation("Отримання продуктів фермера {FarmerId}", farmerId);
            var products = await _repository.GetByFarmerIdAsync(farmerId);
            return products.Select(MapToResponseDto);
        }

        private static ProductResponseDto MapToResponseDto(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Unit = product.Unit,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            FarmerId = product.FarmerId
        };
    }
}
