using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int itemId, string itemType)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var existing = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId && f.ItemType == itemType);

        bool isFavorite;
        if (existing != null) {
            _context.Favorites.Remove(existing);
            isFavorite = false;
        } else {
            _context.Favorites.Add(new Favorite { UserId = userId, ItemId = itemId, ItemType = itemType });
            isFavorite = true;
        }
        await _context.SaveChangesAsync();
        return Json(new { success = true, isFavorite, message = isFavorite ? "Додано в обране ❤️" : "Видалено з обраного 🤍" });
    }

    [Authorize]
    public async Task<IActionResult> Favorites()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var favs = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
        var tractorIds = favs.Where(f => f.ItemType == "Tractor").Select(f => f.ItemId).ToList();
        var trailerIds = favs.Where(f => f.ItemType == "Trailer").Select(f => f.ItemId).ToList();
        var productIds = favs.Where(f => f.ItemType == "Product").Select(f => f.ItemId).ToList();

        var model = new SearchViewModel {
            Query = string.Empty,
            Tractors = tractorIds.Count > 0 ? await _context.Tractors.Where(t => tractorIds.Contains(t.Id)).ToListAsync() : new List<Tractor>(),
            Trailers = trailerIds.Count > 0 ? await _context.Trailers.Where(t => trailerIds.Contains(t.Id)).ToListAsync() : new List<Trailer>(),
            Products = productIds.Count > 0 ? await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync() : new List<Product>()
        };
        return View(model);
    }

    public IActionResult Index() => View();

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    public IActionResult Services() => View();

    public IActionResult Privacy() => View();

    public async Task<IActionResult> Search(string query)
    {
        query = query?.Trim() ?? string.Empty;
        var model = new SearchViewModel
        {
            Query = query,
            Tractors = string.IsNullOrEmpty(query) ? new List<Tractor>() : await _context.Tractors.Where(t => t.Name.Contains(query) || t.Brand.Contains(query)).ToListAsync(),
            Trailers = string.IsNullOrEmpty(query) ? new List<Trailer>() : await _context.Trailers.Where(t => t.Name.Contains(query) || t.Type.Contains(query)).ToListAsync(),
            Products = string.IsNullOrEmpty(query) ? new List<Product>() : await _context.Products.Where(p => p.Name.Contains(query)).ToListAsync()
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
