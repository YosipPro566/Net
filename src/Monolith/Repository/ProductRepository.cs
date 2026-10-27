using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalWebsite.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAvailableAsync()
        {
            return await _dbSet.Where(p => p.IsAvailable).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByFarmerIdAsync(string farmerId)
        {
            return await _dbSet.Where(p => p.FarmerId == farmerId).ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(p => p.Id == id);
        }
    }
}
