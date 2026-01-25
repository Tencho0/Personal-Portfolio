using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Models.ViewModels;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public SettingsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.SiteSettings.ToDictionaryAsync(s => s.Key, s => s.Value ?? string.Empty);

        var viewModel = new SettingsViewModel
        {
            ProfileName = settings.GetValueOrDefault("ProfileName", ""),
            ProfileTitle = settings.GetValueOrDefault("ProfileTitle", ""),
            ProfileBio = settings.GetValueOrDefault("ProfileBio", ""),
            ProfileImagePath = settings.GetValueOrDefault("ProfileImagePath", ""),
            ContactEmail = settings.GetValueOrDefault("ContactEmail", ""),
            ContactPhone = settings.GetValueOrDefault("ContactPhone", "")
        };

        ViewBag.Metrics = await _context.Metrics.OrderBy(m => m.Position).ToListAsync();
        ViewBag.TechStack = await _context.TechStackItems.OrderBy(t => t.Position).ToListAsync();
        ViewBag.SocialLinks = await _context.SocialLinks.OrderBy(s => s.Position).ToListAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model, IFormFile? profileImage)
    {
        if (ModelState.IsValid)
        {
            if (profileImage != null)
            {
                model.ProfileImagePath = await SaveImageAsync(profileImage, "profile");
            }

            await UpdateSettingAsync("ProfileName", model.ProfileName);
            await UpdateSettingAsync("ProfileTitle", model.ProfileTitle);
            await UpdateSettingAsync("ProfileBio", model.ProfileBio);
            await UpdateSettingAsync("ContactEmail", model.ContactEmail);
            await UpdateSettingAsync("ContactPhone", model.ContactPhone);

            if (!string.IsNullOrEmpty(model.ProfileImagePath))
            {
                await UpdateSettingAsync("ProfileImagePath", model.ProfileImagePath);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Metrics = await _context.Metrics.OrderBy(m => m.Position).ToListAsync();
        ViewBag.TechStack = await _context.TechStackItems.OrderBy(t => t.Position).ToListAsync();
        ViewBag.SocialLinks = await _context.SocialLinks.OrderBy(s => s.Position).ToListAsync();

        return View(model);
    }

    #region Metrics

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMetric(string label, string value, string? icon)
    {
        var maxPosition = await _context.Metrics.MaxAsync(m => (int?)m.Position) ?? 0;
        var metric = new Metric
        {
            Label = label,
            Value = value,
            Icon = icon,
            Position = maxPosition + 1
        };
        _context.Metrics.Add(metric);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Metric added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMetric(int id)
    {
        var metric = await _context.Metrics.FindAsync(id);
        if (metric != null)
        {
            metric.IsDeleted = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Metric deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Tech Stack

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTechStack(string name, string? url, IFormFile? logo)
    {
        var maxPosition = await _context.TechStackItems.MaxAsync(t => (int?)t.Position) ?? 0;
        var tech = new TechStackItem
        {
            Name = name,
            Url = url,
            Position = maxPosition + 1
        };

        if (logo != null)
        {
            tech.LogoPath = await SaveImageAsync(logo, "techstack");
        }

        _context.TechStackItems.Add(tech);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Technology added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTechStack(int id)
    {
        var tech = await _context.TechStackItems.FindAsync(id);
        if (tech != null)
        {
            tech.IsDeleted = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Technology deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Social Links

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSocialLink(string platform, string url, string? icon)
    {
        var maxPosition = await _context.SocialLinks.MaxAsync(s => (int?)s.Position) ?? 0;
        var social = new SocialLink
        {
            Platform = platform,
            Url = url,
            Icon = icon,
            Position = maxPosition + 1
        };
        _context.SocialLinks.Add(social);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Social link added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSocialLink(int id)
    {
        var social = await _context.SocialLinks.FindAsync(id);
        if (social != null)
        {
            social.IsDeleted = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Social link deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    #endregion

    private async Task UpdateSettingAsync(string key, string? value)
    {
        var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting != null)
        {
            setting.Value = value;
        }
        else
        {
            _context.SiteSettings.Add(new SiteSetting { Key = key, Value = value });
        }
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
