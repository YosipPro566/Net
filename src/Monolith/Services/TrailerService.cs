using AgriculturalWebsite.DTOs;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Repositories.Interfaces;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Services
{
    public class TrailerService : ITrailerService
    {
        private readonly ITrailerRepository _repository;
        private readonly ILogger<TrailerService> _logger;

        public TrailerService(ITrailerRepository repository, ILogger<TrailerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TrailerResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Отримання всіх причепів");
            var trailers = await _repository.GetAvailableAsync();
            return trailers.Select(MapToResponseDto);
        }

        public async Task<TrailerResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Отримання причепа з ID: {Id}", id);
            var trailer = await _repository.GetByIdAsync(id);
            if (trailer == null)
            {
                _logger.LogWarning("Причіп з ID {Id} не знайдено", id);
                return null;
            }
            return MapToResponseDto(trailer);
        }

        public async Task<TrailerResponseDto> CreateAsync(TrailerCreateDto dto, string farmerId)
        {
            _logger.LogInformation("Створення нового причепа фермером {FarmerId}", farmerId);
            var trailer = new Trailer
            {
                Name = dto.Name,
                Type = dto.Type,
                Capacity = dto.Capacity,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                FarmerId = farmerId,
                IsAvailable = true
            };
            var created = await _repository.CreateAsync(trailer);
            _logger.LogInformation("Причіп створено з ID: {Id}", created.Id);
            return MapToResponseDto(created);
        }

        public async Task<TrailerResponseDto?> UpdateAsync(TrailerUpdateDto dto)
        {
            _logger.LogInformation("Оновлення причепа з ID: {Id}", dto.Id);
            var trailer = await _repository.GetByIdAsync(dto.Id);
            if (trailer == null)
            {
                _logger.LogWarning("Причіп з ID {Id} не знайдено для оновлення", dto.Id);
                return null;
            }
            trailer.Name = dto.Name;
            trailer.Type = dto.Type;
            trailer.Capacity = dto.Capacity;
            trailer.Price = dto.Price;
            trailer.Description = dto.Description;
            trailer.ImageUrl = dto.ImageUrl ?? string.Empty;
            trailer.IsAvailable = dto.IsAvailable;

            var updated = await _repository.UpdateAsync(trailer);
            _logger.LogInformation("Причіп з ID {Id} оновлено", dto.Id);
            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Видалення причепа з ID: {Id}", id);
            var result = await _repository.DeleteAsync(id);
            if (!result) _logger.LogWarning("Причіп з ID {Id} не знайдено для видалення", id);
            return result;
        }

        public async Task<IEnumerable<TrailerResponseDto>> GetByFarmerIdAsync(string farmerId)
        {
            _logger.LogInformation("Отримання причепів фермера {FarmerId}", farmerId);
            var trailers = await _repository.GetByFarmerIdAsync(farmerId);
            return trailers.Select(MapToResponseDto);
        }

        private static TrailerResponseDto MapToResponseDto(Trailer trailer) => new()
        {
            Id = trailer.Id,
            Name = trailer.Name,
            Type = trailer.Type,
            Capacity = trailer.Capacity,
            Price = trailer.Price,
            Description = trailer.Description,
            ImageUrl = trailer.ImageUrl,
            IsAvailable = trailer.IsAvailable,
            FarmerId = trailer.FarmerId
        };
    }
}
