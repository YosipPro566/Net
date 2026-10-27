using AgriculturalWebsite.DTOs;

namespace AgriculturalWebsite.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, string farmerId);
        Task<ProductResponseDto?> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductResponseDto>> GetByFarmerIdAsync(string farmerId);
    }
}
