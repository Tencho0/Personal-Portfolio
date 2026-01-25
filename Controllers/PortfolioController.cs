using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;

namespace Portfolio.Controllers;

public class PortfolioController : Controller
{
    private readonly ApplicationDbContext _context;

    public PortfolioController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .Where(p => p.IsPublished)
            .OrderBy(p => p.Position)
            .ToListAsync();

        return View(projects);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (project == null)
            return NotFound();

        return View(project);
    }
}
