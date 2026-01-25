using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using System.Text.RegularExpressions;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CertificatesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CertificatesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var certificates = await _context.Certificates
            .OrderBy(c => c.Position)
            .ToListAsync();
        return View(certificates);
    }

    public IActionResult Create()
    {
        var maxPosition = _context.Certificates.Max(c => (int?)c.Position) ?? 0;
        return View(new Certificate { Position = maxPosition + 1, DateEarned = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Certificate certificate, IFormFile? image)
    {
        if (string.IsNullOrEmpty(certificate.Slug))
        {
            certificate.Slug = GenerateSlug(certificate.Title);
        }

        if (await _context.Certificates.IgnoreQueryFilters().AnyAsync(c => c.Slug == certificate.Slug))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            if (image != null)
            {
                certificate.ImagePath = await SaveImageAsync(image, "certificates");
            }

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Certificate created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(certificate);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var certificate = await _context.Certificates.FindAsync(id);
        if (certificate == null)
            return NotFound();

        return View(certificate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Certificate certificate, IFormFile? image)
    {
        if (id != certificate.Id)
            return NotFound();

        if (string.IsNullOrEmpty(certificate.Slug))
        {
            certificate.Slug = GenerateSlug(certificate.Title);
        }

        if (await _context.Certificates.IgnoreQueryFilters().AnyAsync(c => c.Slug == certificate.Slug && c.Id != id))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingCert = await _context.Certificates.FindAsync(id);
                if (existingCert == null)
                    return NotFound();

                if (image != null)
                {
                    certificate.ImagePath = await SaveImageAsync(image, "certificates");
                }
                else
                {
                    certificate.ImagePath = existingCert.ImagePath;
                }

                _context.Entry(existingCert).CurrentValues.SetValues(certificate);
                existingCert.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Certificate updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Certificates.AnyAsync(c => c.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(certificate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var certificate = await _context.Certificates.FindAsync(id);
        if (certificate == null)
            return NotFound();

        certificate.IsDeleted = true;
        certificate.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Certificate deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePositions([FromBody] List<int> ids)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            var certificate = await _context.Certificates.FindAsync(ids[i]);
            if (certificate != null)
            {
                certificate.Position = i + 1;
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

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        return slug;
    }
}
