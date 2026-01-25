using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models.ViewModels;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            ProjectCount = await _context.Projects.CountAsync(),
            BlogPostCount = await _context.BlogPosts.CountAsync(),
            ReviewCount = await _context.Reviews.CountAsync(),
            CertificateCount = await _context.Certificates.CountAsync(),
            AwardCount = await _context.Awards.CountAsync(),
            UnreadMessagesCount = await _context.ContactMessages.CountAsync(m => !m.IsRead),
            LatestMessages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToListAsync()
        };

        return View(viewModel);
    }
}
