using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Repositories.Interfaces;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Services
{
    public class TractorService : ITractorService
    {
        private readonly ITractorRepository _repository;
        private readonly ILogger<TractorService> _logger;

        public TractorService(ITractorRepository repository, ILogger<TractorService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TractorResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Отримання всіх тракторів");
            var tractors = await _repository.GetAvailableAsync();
            return tractors.Select(MapToResponseDto);
        }

        public async Task<TractorResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Отримання трактора з ID: {Id}", id);
            var tractor = await _repository.GetByIdAsync(id);
            if (tractor == null)
            {
                _logger.LogWarning("Трактор з ID {Id} не знайдено", id);
                return null;
            }
            return MapToResponseDto(tractor);
        }

        public async Task<TractorResponseDto> CreateAsync(TractorCreateDto dto, string farmerId)
        {
            _logger.LogInformation("Створення нового трактора фермером {FarmerId}", farmerId);
            var tractor = new Tractor
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Model = dto.Model,
                Power = dto.Power,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                FarmerId = farmerId,
                IsAvailable = true
            };
            var created = await _repository.CreateAsync(tractor);
            _logger.LogInformation("Трактор створено з ID: {Id}", created.Id);
            return MapToResponseDto(created);
        }

        public async Task<TractorResponseDto?> UpdateAsync(TractorUpdateDto dto)
        {
            _logger.LogInformation("Оновлення трактора з ID: {Id}", dto.Id);
            var tractor = await _repository.GetByIdAsync(dto.Id);
            if (tractor == null)
            {
                _logger.LogWarning("Трактор з ID {Id} не знайдено для оновлення", dto.Id);
                return null;
            }
            tractor.Name = dto.Name;
            tractor.Brand = dto.Brand;
            tractor.Model = dto.Model;
            tractor.Power = dto.Power;
            tractor.Price = dto.Price;
            tractor.Description = dto.Description;
            tractor.ImageUrl = dto.ImageUrl ?? string.Empty;
            tractor.IsAvailable = dto.IsAvailable;

            var updated = await _repository.UpdateAsync(tractor);
            _logger.LogInformation("Трактор з ID {Id} оновлено", dto.Id);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Видалення трактора з ID: {Id}", id);
            var result = await _repository.DeleteAsync(id);
            if (!result) _logger.LogWarning("Трактор з ID {Id} не знайдено для видалення", id);
            return result;
        }

        public async Task<IEnumerable<TractorResponseDto>> GetByFarmerIdAsync(string farmerId)
        {
            _logger.LogInformation("Отримання тракторів фермера {FarmerId}", farmerId);
            var tractors = await _repository.GetByFarmerIdAsync(farmerId);
            return tractors.Select(MapToResponseDto);
        }

        private static TractorResponseDto MapToResponseDto(Tractor tractor) => new()
        {
            Id = tractor.Id,
            Name = tractor.Name,
            Brand = tractor.Brand,
            Model = tractor.Model,
            Power = tractor.Power,
            Price = tractor.Price,
            Description = tractor.Description,
            ImageUrl = tractor.ImageUrl,
            IsAvailable = tractor.IsAvailable,
            FarmerId = tractor.FarmerId
        };
    }
}
