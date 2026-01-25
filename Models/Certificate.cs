using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class Certificate : SortableEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public DateTime DateEarned { get; set; }

    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    [StringLength(500)]
    [Url]
    public string? VerificationUrl { get; set; }
}
