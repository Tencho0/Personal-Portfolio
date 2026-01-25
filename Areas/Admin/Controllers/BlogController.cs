using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Helpers;
using Portfolio.Models;
using System.Text.RegularExpressions;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BlogController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View(new BlogPost());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPost post, IFormFile? coverImage)
    {
        if (string.IsNullOrEmpty(post.Slug))
        {
            post.Slug = GenerateSlug(post.Title);
        }

        if (await _context.BlogPosts.IgnoreQueryFilters().AnyAsync(p => p.Slug == post.Slug))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            if (coverImage != null)
            {
                post.CoverImagePath = await SaveImageAsync(coverImage, "blog");
            }

            if (post.Status == PostStatus.Published && !post.PublishedAt.HasValue)
            {
                post.PublishedAt = DateTime.UtcNow;
            }

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Blog post created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null)
            return NotFound();

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPost post, IFormFile? coverImage)
    {
        if (id != post.Id)
            return NotFound();

        if (string.IsNullOrEmpty(post.Slug))
        {
            post.Slug = GenerateSlug(post.Title);
        }

        if (await _context.BlogPosts.IgnoreQueryFilters().AnyAsync(p => p.Slug == post.Slug && p.Id != id))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingPost = await _context.BlogPosts.FindAsync(id);
                if (existingPost == null)
                    return NotFound();

                // Handle publishing
                if (post.Status == PostStatus.Published && !existingPost.PublishedAt.HasValue)
                {
                    post.PublishedAt = DateTime.UtcNow;
                }
                else
                {
                    post.PublishedAt = existingPost.PublishedAt;
                }

                if (coverImage != null)
                {
                    post.CoverImagePath = await SaveImageAsync(coverImage, "blog");
                }
                else
                {
                    post.CoverImagePath = existingPost.CoverImagePath;
                }

                _context.Entry(existingPost).CurrentValues.SetValues(post);
                existingPost.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Blog post updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.BlogPosts.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null)
            return NotFound();

        post.IsDeleted = true;
        post.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Blog post deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult PreviewMarkdown([FromBody] string markdown)
    {
        var html = MarkdownHelper.ToHtml(markdown);
        return Content(html, "text/html");
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
