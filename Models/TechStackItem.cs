using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class TechStackItem : SortableEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? LogoPath { get; set; }

    [StringLength(500)]
    [Url]
    public string? Url { get; set; }
}
