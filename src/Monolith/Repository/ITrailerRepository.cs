using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Repositories.Interfaces
{
    public interface ITrailerRepository : IRepository<Trailer>
    {
        Task<IEnumerable<Trailer>> GetAvailableAsync();
        Task<IEnumerable<Trailer>> GetByFarmerIdAsync(string farmerId);
        Task<bool> ExistsAsync(int id);
    }
}
