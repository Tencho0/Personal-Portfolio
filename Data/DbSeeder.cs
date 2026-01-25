using Microsoft.AspNetCore.Identity;
using Portfolio.Models;

namespace Portfolio.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create admin user if it doesn't exist
        var adminEmail = configuration["Admin:Email"] ?? "admin@portfolio.com";
        var adminPassword = configuration["Admin:Password"] ?? "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed default site settings
        await SeedSiteSettingsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedSiteSettingsAsync(ApplicationDbContext context)
    {
        var defaultSettings = new Dictionary<string, string>
        {
            { "ProfileName", "Мистър Перфектен" },
            { "ProfileTitle", "Software Engineer" },
            { "ProfileBio", "I am a passionate software engineer specializing in building modern web applications. With years of experience in full-stack development, I help businesses transform their ideas into reality." },
            { "ProfileImagePath", "/images/profile-placeholder.png" },
            { "ContactEmail", "contact@example.com" },
            { "ContactPhone", "+1 (555) 123-4567" }
        };

        foreach (var setting in defaultSettings)
        {
            if (!context.SiteSettings.Any(s => s.Key == setting.Key))
            {
                context.SiteSettings.Add(new SiteSetting
                {
                    Key = setting.Key,
                    Value = setting.Value
                });
            }
        }

        // Seed default metrics
        if (!context.Metrics.Any())
        {
            var metrics = new[]
            {
                new Metric { Label = "Projects Completed", Value = "50+", Position = 1, Icon = "bi-folder-check" },
                new Metric { Label = "Happy Clients", Value = "30+", Position = 2, Icon = "bi-emoji-smile" },
                new Metric { Label = "Years Experience", Value = "5+", Position = 3, Icon = "bi-calendar-check" },
                new Metric { Label = "Technologies", Value = "20+", Position = 4, Icon = "bi-code-slash" }
            };
            context.Metrics.AddRange(metrics);
        }

        // Seed default social links
        if (!context.SocialLinks.Any())
        {
            var socialLinks = new[]
            {
                new SocialLink { Platform = "LinkedIn", Url = "https://linkedin.com/in/example", Icon = "bi-linkedin", Position = 1 },
                new SocialLink { Platform = "GitHub", Url = "https://github.com/example", Icon = "bi-github", Position = 2 },
                new SocialLink { Platform = "Twitter", Url = "https://twitter.com/example", Icon = "bi-twitter-x", Position = 3 }
            };
            context.SocialLinks.AddRange(socialLinks);
        }

        // Seed default tech stack items
        if (!context.TechStackItems.Any())
        {
            var techStack = new[]
            {
                new TechStackItem { Name = "C#", Position = 1 },
                new TechStackItem { Name = ".NET", Position = 2 },
                new TechStackItem { Name = "SQL Server", Position = 3 },
                new TechStackItem { Name = "JavaScript", Position = 4 },
                new TechStackItem { Name = "TypeScript", Position = 5 },
                new TechStackItem { Name = "React", Position = 6 },
                new TechStackItem { Name = "Azure", Position = 7 },
                new TechStackItem { Name = "Docker", Position = 8 }
            };
            context.TechStackItems.AddRange(techStack);
        }
    }
}
