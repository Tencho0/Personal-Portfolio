using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;

namespace Portfolio.Controllers;

public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var reviews = await _context.Reviews
            .OrderBy(r => r.Position)
            .ToListAsync();

        return View(reviews);
    }
}
