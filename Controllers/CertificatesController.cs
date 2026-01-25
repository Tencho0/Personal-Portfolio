using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models.ViewModels;

namespace Portfolio.Controllers;

public class CertificatesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CertificatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string tab = "certificates")
    {
        var viewModel = new CertificatesViewModel
        {
            Certificates = await _context.Certificates.OrderBy(c => c.Position).ToListAsync(),
            Awards = await _context.Awards.OrderBy(a => a.Position).ToListAsync(),
            ActiveTab = tab
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (certificate == null)
            return NotFound();

        return View(certificate);
    }
}
