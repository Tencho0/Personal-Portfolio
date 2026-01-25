using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Models;

namespace Portfolio.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Award> Awards => Set<Award>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<TechStackItem> TechStackItems => Set<TechStackItem>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<Metric> Metrics => Set<Metric>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Project configuration
        builder.Entity<Project>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // BlogPost configuration
        builder.Entity<BlogPost>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Review configuration
        builder.Entity<Review>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Certificate configuration
        builder.Entity<Certificate>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Award configuration
        builder.Entity<Award>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ContactMessage configuration
        builder.Entity<ContactMessage>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // TechStackItem configuration
        builder.Entity<TechStackItem>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SocialLink configuration
        builder.Entity<SocialLink>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Metric configuration
        builder.Entity<Metric>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // SiteSetting configuration
        builder.Entity<SiteSetting>(entity =>
        {
            entity.HasIndex(e => e.Key).IsUnique();
        });
    }
}
