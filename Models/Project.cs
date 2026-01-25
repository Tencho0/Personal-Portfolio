using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class Project : SortableEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    public string? LongDescription { get; set; }

    [StringLength(500)]
    public string? Technologies { get; set; }

    [StringLength(500)]
    public string? ThumbnailImagePath { get; set; }

    [StringLength(500)]
    [Url]
    public string? LiveUrl { get; set; }

    [StringLength(500)]
    [Url]
    public string? GithubUrl { get; set; }

    [StringLength(200)]
    public string? ClientName { get; set; }

    public bool IsPublished { get; set; } = false;
}
