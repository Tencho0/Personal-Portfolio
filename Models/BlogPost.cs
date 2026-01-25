using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public enum PostStatus
{
    Draft,
    Published
}

public class BlogPost : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [Required]
    public string MarkdownContent { get; set; } = string.Empty;

    [StringLength(500)]
    public string? PreviewText { get; set; }

    public PostStatus Status { get; set; } = PostStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    [StringLength(500)]
    public string? CoverImagePath { get; set; }
}
