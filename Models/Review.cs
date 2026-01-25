using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class Review : SortableEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? PhotoPath { get; set; }

    [StringLength(200)]
    public string? Source { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = string.Empty;

    [Range(1, 5)]
    public int? StarRating { get; set; }

    public bool Featured { get; set; } = false;
}
