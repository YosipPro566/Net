using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Data;

namespace AgriculturalWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int itemId, string itemType)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var existing = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId && f.ItemType == itemType);

        if (existing != null) {
            _context.Favorites.Remove(existing);
        } else {
            _context.Favorites.Add(new Favorite { UserId = userId, ItemId = itemId, ItemType = itemType });
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> Favorites()
    {
        var userId = _userManager.GetUserId(User);
        var favs = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
        
        var model = new SearchViewModel {
            Tractors = await _context.Tractors.Where(t => favs.Any(f => f.ItemId == t.Id && f.ItemType == "Tractor")).ToListAsync(),
            Trailers = await _context.Trailers.Where(t => favs.Any(f => f.ItemId == t.Id && f.ItemType == "Trailer")).ToListAsync(),
            Products = await _context.Products.Where(p => favs.Any(f => f.ItemId == p.Id && f.ItemType == "Product")).ToListAsync()
        };
        return View(model);
    }
    
    // Решта методів Index, Privacy тощо залишаються як є
}