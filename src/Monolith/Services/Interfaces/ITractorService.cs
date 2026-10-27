using AgriculturalWebsite.DTOs;

namespace AgriculturalWebsite.Services.Interfaces
{
    public interface ITractorService
    {
        Task<IEnumerable<TractorResponseDto>> GetAllAsync();
        Task<TractorResponseDto?> GetByIdAsync(int id);
        Task<TractorResponseDto> CreateAsync(TractorCreateDto dto, string farmerId);
        Task<TractorResponseDto?> UpdateAsync(TractorUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TractorResponseDto>> GetByFarmerIdAsync(string farmerId);
    }
}
