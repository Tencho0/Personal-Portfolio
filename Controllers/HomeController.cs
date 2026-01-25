using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Models.ViewModels;
using System.Diagnostics;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.SiteSettings.ToDictionaryAsync(s => s.Key, s => s.Value ?? string.Empty);

        var viewModel = new HomeViewModel
        {
            ProfileName = settings.GetValueOrDefault("ProfileName", ""),
            ProfileTitle = settings.GetValueOrDefault("ProfileTitle", ""),
            ProfileBio = settings.GetValueOrDefault("ProfileBio", ""),
            ProfileImagePath = settings.GetValueOrDefault("ProfileImagePath", "/images/profile-placeholder.png"),
            Metrics = await _context.Metrics.OrderBy(m => m.Position).ToListAsync(),
            TechStack = await _context.TechStackItems.OrderBy(t => t.Position).ToListAsync(),
            FeaturedReviews = await _context.Reviews
                .OrderByDescending(r => r.Featured)
                .ThenBy(r => r.Position)
                .Take(3)
                .ToListAsync(),
            LatestPosts = await _context.BlogPosts
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt)
                .Take(3)
                .ToListAsync()
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
