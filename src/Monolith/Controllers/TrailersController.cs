using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using Microsoft.AspNetCore.Authorization;

namespace AgriculturalWebsite.Controllers
{
    public class TrailersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrailersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trailers
        public async Task<IActionResult> Index()
        {
            var trailers = await _context.Trailers.Where(t => t.IsAvailable).ToListAsync();
            return View(trailers);
        }

        // GET: Trailers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers.FirstOrDefaultAsync(m => m.Id == id);
            if (trailer == null)
            {
                return NotFound();
            }

            // ПЕРЕДАЄМО FARMER_ID ДЛЯ ЧАТУ
            ViewData["SellerId"] = trailer.FarmerId ?? "";

            return View(trailer);
        }

        // GET: Trailers/Create
        [Authorize(Roles = "Admin,Farmer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trailers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Create([Bind("Name,Type,Capacity,Price,Description,ImageUrl")] Trailer trailer)
        {
            if (ModelState.IsValid)
            {
                trailer.FarmerId = User.Identity?.Name;
                _context.Add(trailer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trailer);
        }

        // GET: Trailers/Edit/5
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers.FindAsync(id);
            if (trailer == null)
            {
                return NotFound();
            }
            return View(trailer);
        }

        // POST: Trailers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Capacity,Price,Description,ImageUrl,IsAvailable")] Trailer trailer)
        {
            if (id != trailer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trailer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrailerExists(trailer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trailer);
        }

        // GET: Trailers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers.FirstOrDefaultAsync(m => m.Id == id);
            if (trailer == null)
            {
                return NotFound();
            }

            return View(trailer);
        }

        // POST: Trailers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trailer = await _context.Trailers.FindAsync(id);
            if (trailer != null)
            {
                _context.Trailers.Remove(trailer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrailerExists(int id)
        {
            return _context.Trailers.Any(e => e.Id == id);
        }
    }
}