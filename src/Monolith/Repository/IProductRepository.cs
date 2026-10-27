using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAvailableAsync();
        Task<IEnumerable<Product>> GetByFarmerIdAsync(string farmerId);
        Task<bool> ExistsAsync(int id);
    }
}
