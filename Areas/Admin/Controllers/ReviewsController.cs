using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ReviewsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var reviews = await _context.Reviews
            .OrderBy(r => r.Position)
            .ToListAsync();
        return View(reviews);
    }

    public IActionResult Create()
    {
        var maxPosition = _context.Reviews.Max(r => (int?)r.Position) ?? 0;
        return View(new Review { Position = maxPosition + 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review review, IFormFile? photo)
    {
        if (ModelState.IsValid)
        {
            if (photo != null)
            {
                review.PhotoPath = await SaveImageAsync(photo, "reviews");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Review created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(review);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Review review, IFormFile? photo)
    {
        if (id != review.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingReview = await _context.Reviews.FindAsync(id);
                if (existingReview == null)
                    return NotFound();

                if (photo != null)
                {
                    review.PhotoPath = await SaveImageAsync(photo, "reviews");
                }
                else
                {
                    review.PhotoPath = existingReview.PhotoPath;
                }

                _context.Entry(existingReview).CurrentValues.SetValues(review);
                existingReview.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Review updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Reviews.AnyAsync(r => r.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        review.IsDeleted = true;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Review deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePositions([FromBody] List<int> ids)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            var review = await _context.Reviews.FindAsync(ids[i]);
            if (review != null)
            {
                review.Position = i + 1;
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
