using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AwardsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AwardsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var awards = await _context.Awards
            .OrderBy(a => a.Position)
            .ToListAsync();
        return View(awards);
    }

    public IActionResult Create()
    {
        var maxPosition = _context.Awards.Max(a => (int?)a.Position) ?? 0;
        return View(new Award { Position = maxPosition + 1, DateReceived = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Award award, IFormFile? image)
    {
        if (ModelState.IsValid)
        {
            if (image != null)
            {
                award.ImagePath = await SaveImageAsync(image, "awards");
            }

            _context.Awards.Add(award);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Award created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(award);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var award = await _context.Awards.FindAsync(id);
        if (award == null)
            return NotFound();

        return View(award);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Award award, IFormFile? image)
    {
        if (id != award.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingAward = await _context.Awards.FindAsync(id);
                if (existingAward == null)
                    return NotFound();

                if (image != null)
                {
                    award.ImagePath = await SaveImageAsync(image, "awards");
                }
                else
                {
                    award.ImagePath = existingAward.ImagePath;
                }

                _context.Entry(existingAward).CurrentValues.SetValues(award);
                existingAward.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Award updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Awards.AnyAsync(a => a.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(award);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var award = await _context.Awards.FindAsync(id);
        if (award == null)
            return NotFound();

        award.IsDeleted = true;
        award.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Award deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePositions([FromBody] List<int> ids)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            var award = await _context.Awards.FindAsync(ids[i]);
            if (award != null)
            {
                award.Position = i + 1;
            }
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    private async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/{folder}/{uniqueFileName}";
    }
}
