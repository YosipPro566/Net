using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalWebsite.Repositories
{
    public class TrailerRepository : Repository<Trailer>, ITrailerRepository
    {
        public TrailerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Trailer>> GetAvailableAsync()
        {
            return await _dbSet.Where(t => t.IsAvailable).ToListAsync();
        }

        public async Task<IEnumerable<Trailer>> GetByFarmerIdAsync(string farmerId)
        {
            return await _dbSet.Where(t => t.FarmerId == farmerId).ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(t => t.Id == id);
        }
    }
}
