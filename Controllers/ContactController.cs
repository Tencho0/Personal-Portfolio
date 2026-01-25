using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Models.ViewModels;

namespace Portfolio.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.SiteSettings.ToDictionaryAsync(s => s.Key, s => s.Value ?? string.Empty);

        var viewModel = new ContactViewModel
        {
            Email = settings.GetValueOrDefault("ContactEmail", ""),
            Phone = settings.GetValueOrDefault("ContactPhone", ""),
            SocialLinks = await _context.SocialLinks.OrderBy(s => s.Position).ToListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        var settings = await _context.SiteSettings.ToDictionaryAsync(s => s.Key, s => s.Value ?? string.Empty);

        model.Email = settings.GetValueOrDefault("ContactEmail", "");
        model.Phone = settings.GetValueOrDefault("ContactPhone", "");
        model.SocialLinks = await _context.SocialLinks.OrderBy(s => s.Position).ToListAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = new ContactMessage
        {
            Name = model.Form.Name,
            Email = model.Form.Email,
            Message = model.Form.Message
        };

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();

        model.MessageSent = true;
        model.Form = new ContactFormModel();
        ModelState.Clear();

        return View(model);
    }
}
