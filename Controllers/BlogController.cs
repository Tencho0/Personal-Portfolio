using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfolio.Controllers;

public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;

    public BlogController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts
            .Where(p => p.Status == PostStatus.Published)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        return View(posts);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var post = await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PostStatus.Published);

        if (post == null)
            return NotFound();

        return View(post);
    }
}
