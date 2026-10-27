using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using Microsoft.AspNetCore.Authorization;

namespace AgriculturalWebsite.Controllers
{
    public class TractorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TractorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tractors
        public async Task<IActionResult> Index()
        {
            var tractors = await _context.Tractors.Where(t => t.IsAvailable).ToListAsync();
            return View(tractors);
        }

        // GET: Tractors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tractor = await _context.Tractors.FirstOrDefaultAsync(m => m.Id == id);
            if (tractor == null)
            {
                return NotFound();
            }

            // ПЕРЕДАЄМО FARMER_ID ДЛЯ ЧАТУ
            ViewData["SellerId"] = tractor.FarmerId ?? "";

            return View(tractor);
        }

        // GET: Tractors/Create
        [Authorize(Roles = "Admin,Farmer")]
        public IActionResult Create()
        {
            return View(new Tractor());
        }

        // POST: Tractors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Create([Bind("Name,Brand,Model,Power,Price,Description,ImageUrl")] Tractor tractor)
        {
            if (ModelState.IsValid)
            {
                tractor.FarmerId = User.Identity?.Name;
                _context.Add(tractor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tractor);
        }

        // GET: Tractors/Edit/5
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tractor = await _context.Tractors.FindAsync(id);
            if (tractor == null)
            {
                return NotFound();
            }
            return View(tractor);
        }

        // POST: Tractors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Model,Power,Price,Description,ImageUrl,IsAvailable")] Tractor tractor)
        {
            if (id != tractor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tractor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TractorExists(tractor.Id))
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
            return View(tractor);
        }

        // GET: Tractors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tractor = await _context.Tractors.FirstOrDefaultAsync(m => m.Id == id);
            if (tractor == null)
            {
                return NotFound();
            }

            return View(tractor);
        }

        // POST: Tractors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tractor = await _context.Tractors.FindAsync(id);
            if (tractor != null)
            {
                _context.Tractors.Remove(tractor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TractorExists(int id)
        {
            return _context.Tractors.Any(e => e.Id == id);
        }
    }
}