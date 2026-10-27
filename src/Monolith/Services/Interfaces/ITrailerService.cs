using AgriculturalWebsite.DTOs;

namespace AgriculturalWebsite.Services.Interfaces
{
    public interface ITrailerService
    {
        Task<IEnumerable<TrailerResponseDto>> GetAllAsync();
        Task<TrailerResponseDto?> GetByIdAsync(int id);
        Task<TrailerResponseDto> CreateAsync(TrailerCreateDto dto, string farmerId);
        Task<TrailerResponseDto?> UpdateAsync(TrailerUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TrailerResponseDto>> GetByFarmerIdAsync(string farmerId);
    }
}
