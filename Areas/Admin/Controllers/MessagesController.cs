using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MessagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return View(messages);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
            return NotFound();

        if (!message.IsRead)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
            return NotFound();

        message.IsRead = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsUnread(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
            return NotFound();

        message.IsRead = false;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
            return NotFound();

        message.IsDeleted = true;
        message.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Message deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
