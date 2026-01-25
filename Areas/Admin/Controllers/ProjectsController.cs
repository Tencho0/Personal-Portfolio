using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using System.Text.RegularExpressions;

namespace Portfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProjectsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .OrderBy(p => p.Position)
            .ToListAsync();
        return View(projects);
    }

    public IActionResult Create()
    {
        var maxPosition = _context.Projects.Max(p => (int?)p.Position) ?? 0;
        return View(new Project { Position = maxPosition + 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project, IFormFile? thumbnailImage)
    {
        if (string.IsNullOrEmpty(project.Slug))
        {
            project.Slug = GenerateSlug(project.Title);
        }

        if (await _context.Projects.IgnoreQueryFilters().AnyAsync(p => p.Slug == project.Slug))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            if (thumbnailImage != null)
            {
                project.ThumbnailImagePath = await SaveImageAsync(thumbnailImage, "projects");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Project created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(project);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project, IFormFile? thumbnailImage)
    {
        if (id != project.Id)
            return NotFound();

        if (string.IsNullOrEmpty(project.Slug))
        {
            project.Slug = GenerateSlug(project.Title);
        }

        if (await _context.Projects.IgnoreQueryFilters().AnyAsync(p => p.Slug == project.Slug && p.Id != id))
        {
            ModelState.AddModelError("Slug", "This slug is already taken.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingProject = await _context.Projects.FindAsync(id);
                if (existingProject == null)
                    return NotFound();

                if (thumbnailImage != null)
                {
                    project.ThumbnailImagePath = await SaveImageAsync(thumbnailImage, "projects");
                }
                else
                {
                    project.ThumbnailImagePath = existingProject.ThumbnailImagePath;
                }

                _context.Entry(existingProject).CurrentValues.SetValues(project);
                existingProject.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Project updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Projects.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.IsDeleted = true;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Project deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePositions([FromBody] List<int> ids)
    {
        for (int i = 0; i < ids.Count; i++)
        {
            var project = await _context.Projects.FindAsync(ids[i]);
            if (project != null)
            {
                project.Position = i + 1;
            }
        }
        await _context.SaveChangesAsync();
        return Ok();
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
