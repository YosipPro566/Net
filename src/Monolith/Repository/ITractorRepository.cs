using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Repositories.Interfaces
{
    public interface ITractorRepository : IRepository<Tractor>
    {
        Task<IEnumerable<Tractor>> GetAvailableAsync();
        Task<IEnumerable<Tractor>> GetByFarmerIdAsync(string farmerId);
        Task<bool> ExistsAsync(int id);
    }
}
